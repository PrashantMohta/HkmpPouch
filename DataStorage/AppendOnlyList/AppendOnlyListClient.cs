using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HkmpPouch.Packets;
using Newtonsoft.Json;

namespace HkmpPouch.DataStorage.AppendOnlyList
{
    public class AppendOnlyListClient
    {
        public string Name;
        public List<string> listData = new();
        public event EventHandler<AppendOnlyListUpdateEventArgs> OnUpdate;
        private PipeClient pipe;

        public AppendOnlyListClient(PipeClient pipe, string name)
        {
            this.pipe = pipe;
            this.Name = name;
            pipe.OnRecieve += HandleEvent;
        }

        public void HandleEvent(System.Object _, RecievedEventArgs args)
        {
            var packet = args.Data;
            if (!packet.EventData.StartsWith($"{Name}|")) { return; }
            if (packet.EventName == AppendOnlyListEvents.ADDED)
            {
                var data = packet.EventData.Split(new Char[] { '|' }, 2);
                var item = data[1];
                listData.Add(item);
                OnUpdate?.Invoke(this, new AppendOnlyListUpdateEventArgs
                {
                    data = this.listData
                });

            }
            if (packet.EventName == AppendOnlyListEvents.GOTALL)
            {
                var data = packet.EventData.Split(new Char[] { '|' }, 2);
                listData = JsonConvert.DeserializeObject<List<string>>(data[1]);
                OnUpdate?.Invoke(this, new AppendOnlyListUpdateEventArgs
                {
                    data = this.listData
                });
            }
        }

        public void Add(string item, int ttl)
        {
            // send add command to server
            this.pipe.SendToServer(AppendOnlyListEvents.ADD, $"{this.Name}|{ttl}|{item}");
        }

        public void GetAll()
        {
            // send getAll command to server
            this.pipe.SendToServer(AppendOnlyListEvents.GETALL, this.Name);
        }
        public void Destory()
        {
            pipe.OnRecieve -= HandleEvent;
        }
    }
}
