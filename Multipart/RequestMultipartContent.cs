using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HkmpPouch.Multipart
{
    public class RequestMultipartContent : PipeEvent
    {
        public static string Name = "RequestMultipartContent";
        public ushort PartNumber = 0;
        public string ContentId;

        public RequestMultipartContent(string ContentId)
        {
            base.IsReliable = true;
            this.ContentId = ContentId;
        }
        public override string GetName() => Name;
        public override string ToString()
        {
            return $"{ContentId},{PartNumber}";
        }
    }

    public class RequestMultipartContentFactory : IEventFactory
    {
        public static RequestMultipartContentFactory Instance { get; internal set; } = new RequestMultipartContentFactory();
        public PipeEvent FromSerializedString(string serializedData)
        {
            var Split = serializedData.Split(',');
            var ContentId = Split[0];
            var pEvent = new RequestMultipartContent(ContentId);
            pEvent.PartNumber = ushort.Parse(Split[1], CultureInfo.InvariantCulture);
            return pEvent;
        }

        public string GetName() => RequestMultipartContent.Name;
    }
}
