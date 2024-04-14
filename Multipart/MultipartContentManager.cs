using Satchel;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HkmpPouch.Multipart
{
    internal class OngoingRequest
    {
        public string ContentId;
        public ushort PartNumber;
        public DateTime LastRequest;
    }
    /// <summary>
    /// Allows handling transmission of arbitrary byte[] in parts
    /// </summary>
    public class MultipartContentManager
    {

        private static ushort PartSize = 500; 
        private static double MaxTimeout = 60;

        private Dictionary<string, byte[]> _content = new();
        private Dictionary<string, Dictionary<ushort, byte[]>> _partialContent = new();
        private Dictionary<string, OngoingRequest> _ongoingRequests = new();
        private Timer EventTimer;

        private OnAble pipe;

        /// <summary>
        /// Action that must actually send the content request to the correct client/server
        /// </summary>
        public Action<RequestMultipartContent> SendContentRequest;

        /// <summary>
        /// Action that must handle sending the requested content from the sending side
        /// </summary>
        public Action<RequestedMultipartContent> ContentRequestHandler;

        /// <summary>
        /// Action that is called with the requested data on the receiver side
        /// </summary>
        public Action<RequestedMultipartContent> ContentReceivedHandler;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="pipe"></param>
        public MultipartContentManager(OnAble pipe)
        {
            this.pipe = pipe;
            pipe.On(RequestedMultipartContentFactory.Instance).Do<RequestedMultipartContent>(ReceivedContent);
            pipe.On(RequestMultipartContentFactory.Instance).Do<RequestMultipartContent>(ContentRequested);
            EventTimer = new Timer(1000);
            EventTimer.Elapsed += EventTimer_Elapsed;
        }

        private void EventTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var request in _ongoingRequests) {
                if((DateTime.Now - request.Value.LastRequest).TotalSeconds < MaxTimeout)
                {
                    _ongoingRequests[request.Value.ContentId] = new() { ContentId = request.Value.ContentId, PartNumber = request.Value.PartNumber, LastRequest = DateTime.Now };
                    SendContentRequest(new RequestMultipartContent(request.Value.ContentId) { PartNumber = _ongoingRequests[request.Value.ContentId].PartNumber });
                }
            }
        }

        private void ContentRequested(RequestMultipartContent request)
        {
            if(ContentRequestHandler != null)
            {
                byte[] currentPartContent;
                ushort totalParts = 1;
                if (_partialContent.ContainsKey(request.ContentId) && _partialContent[request.ContentId].ContainsKey(request.PartNumber))
                {
                   currentPartContent = _partialContent[request.ContentId][request.PartNumber];
                } else
                {
                    // get the necessary part & cache
                    if (!_partialContent.ContainsKey(request.ContentId))
                    {
                        _partialContent[request.ContentId] = new();
                    }
                    var data = GetContent(request.ContentId);
                    List<byte> currentBuffer = new List<byte>();
                    if (data != null)
                    {
                        totalParts = (ushort)Math.Ceiling((float)data.Length / PartSize);
                        var currentIndex = request.PartNumber * PartSize;
                        var readPos = 0;
                        while (currentIndex + readPos < data.Length && readPos < PartSize)
                        {
                            currentBuffer.Add(data[currentIndex + readPos]);
                            readPos++;
                        }
                    }
                    currentPartContent = currentBuffer.ToArray();
                    _partialContent[request.ContentId][request.PartNumber] = currentPartContent;

                }
                // send the part to the request handler
                // request, currentPartContent
                ContentRequestHandler(new RequestedMultipartContent(request.ContentId) { 
                    PartNumber = request.PartNumber, 
                    TotalParts = totalParts , 
                    ExtraBytes = currentPartContent});
            }
        }

        private void ReceivedContent(RequestedMultipartContent content)
        {
            //handle recombining of multipart content 
            if (!_partialContent.ContainsKey(content.ContentId))
            {
                _partialContent[content.ContentId] = new();
            }
            if (_partialContent[content.ContentId].ContainsKey(content.PartNumber))
            {
                return;
            }
            _partialContent[content.ContentId][content.PartNumber] = content.ExtraBytes;
            if (_partialContent[content.ContentId].Count == content.TotalParts)
            {
                var totalData = new List<byte>();
                foreach (var part in _partialContent[content.ContentId])
                {
                    totalData.AddRange(part.Value);
                }
                _ongoingRequests.Remove(content.ContentId);
                if (ContentReceivedHandler != null)
                {
                    var data = totalData.ToArray();
                    RegisterContent(content.ContentId, data);
                    ContentReceivedHandler(new RequestedMultipartContent(content.ContentId) {PartNumber = 0 ,TotalParts = 1, ExtraBytes = data});
                }
            }
            else
            {
                if (_ongoingRequests.ContainsKey(content.ContentId))
                {                
                    //request the next pending part
                    ushort nextPart = (ushort)(content.PartNumber + 1);
                    _ongoingRequests[content.ContentId].PartNumber = nextPart;
                    _ongoingRequests[content.ContentId].LastRequest = DateTime.Now;
                    if (nextPart < content.TotalParts && !_partialContent[content.ContentId].ContainsKey(nextPart))
                    {
                        if (SendContentRequest != null)
                        {
                            SendContentRequest(new RequestMultipartContent(content.ContentId) { PartNumber = nextPart });
                        }
                    }
                }
            }

            
        }

        /// <summary>
        /// Used to request the content over the network
        /// </summary>
        /// <param name="contentId"></param>
        public void RequestContent(string contentId)
        {
            if (!_ongoingRequests.ContainsKey(contentId))
            {
                _ongoingRequests[contentId] = new() { ContentId = contentId ,PartNumber = 0, LastRequest = DateTime.Now};
                SendContentRequest(new RequestMultipartContent(contentId) { PartNumber = _ongoingRequests[contentId].PartNumber });
            }
        }
        
        /// <summary>
        /// Used to register content
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="content"></param>
        public void RegisterContent(string contentId, byte[] content)
        {
            _content[contentId] = content;
        }

        /// <summary>
        /// Used to check if content is already available
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public byte[] GetContent(string contentId) {
            if (_content.ContainsKey(contentId))
            {
                return _content[contentId];
            }
            // handle making a request for the content?
            return null;
        }

        /// <summary>
        /// Get the list of content that is available
        /// </summary>
        /// <returns></returns>
        public List<string> GetContentList()
        {
            return _content.Keys.ToList();
        }
    }
}
