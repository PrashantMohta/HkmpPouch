using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HkmpPouch.Multipart
{
    public class RequestedMultipartContent : PipeEvent
    {
        public static string Name = "RequestedMultipartContent";
        public ushort PartNumber = 0;
        public ushort TotalParts = 1;
        public string ContentId;

        public RequestedMultipartContent(string ContentId)
        {
            base.IsReliable = true;
            this.ContentId = ContentId;
        }
        public override string GetName() => Name;
        public override string ToString()
        {
            return $"{ContentId},{PartNumber},{TotalParts}";
        }
    }
    public class RequestedMultipartContentFactory : IEventFactory
    {
        public static RequestedMultipartContentFactory Instance { get; internal set; } = new RequestedMultipartContentFactory();
        public PipeEvent FromSerializedString(string serializedData)
        {
            var Split = serializedData.Split(',');
            var ContentId = Split[0];
            var pEvent = new RequestedMultipartContent(ContentId);
            pEvent.PartNumber = ushort.Parse(Split[1], CultureInfo.InvariantCulture);
            pEvent.TotalParts = ushort.Parse(Split[2], CultureInfo.InvariantCulture);
            return pEvent;
        }

        public string GetName() => RequestedMultipartContent.Name;
    }
}
