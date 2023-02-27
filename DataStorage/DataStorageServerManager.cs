using HkmpPouch.DataStorage.AppendOnlyList;
using HkmpPouch.DataStorage.Counter;
using HkmpPouch.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HkmpPouch.DataStorage
{
    public class DataStorageServerManager
    {

        public string modName;
        private Dictionary<string, CounterServer> Counters = new();
        private Dictionary<string, AppendOnlyListServer> AppendOnlyLists = new();
        private PipeServer pipe;
        public DataStorageServerManager(string modName)
        {
            this.modName = modName;
            this.pipe = new PipeServer(this.modName);
            this.pipe.OnRecieve += HandleEvent;
        }
        public void HandleEvent(System.Object _, RecievedEventArgs args)
        {

            if (Counters == null)
            {
                Counters = new();
            }
            if (AppendOnlyLists == null)
            {
                AppendOnlyLists = new();
            }

            var packet = args.Data;
            Server.Instance.Info($"{modName} PouchData recieve {packet.EventName} = {packet.EventData}");

            if (packet.EventName == CounterEvents.INCREMENT || packet.EventName == CounterEvents.DECREMENT || packet.EventName == CounterEvents.GET)
            {
                CounterServer c;
                var data = packet.EventData.Split(new Char[] { '|' }, 2);
                var counterName = data[0];
                if (!Counters.TryGetValue(counterName, out c))
                {
                    c = new CounterServer(pipe, counterName);
                    Counters[counterName] = c;
                }
                if (packet.EventName == CounterEvents.GET)
                {
                    c.UpdateClient(packet.FromPlayer);
                }
                if (packet.EventName == CounterEvents.INCREMENT)
                {
                    c.Increment(ushort.Parse(data[1]));
                }
                if (packet.EventName == CounterEvents.DECREMENT)
                {
                    c.Decrement(ushort.Parse(data[1]));
                }
            }
            if (packet.EventName == AppendOnlyListEvents.ADD || packet.EventName == AppendOnlyListEvents.GETALL)
            {
                AppendOnlyListServer a;
                var data = packet.EventData.Split(new Char[] { '|' }, 3);
                var listName = data[0];
                if (!AppendOnlyLists.TryGetValue(listName, out a))
                {
                    a = new AppendOnlyListServer(pipe, listName);
                    AppendOnlyLists[listName] = a;
                }
                if (packet.EventName == AppendOnlyListEvents.ADD)
                {
                    a.Add(new ListItem
                    {
                        insertedOn = DateTime.Now,
                        ttl = Int32.Parse(data[1]),
                        value = data[2]
                    });
                }
                if (packet.EventName == AppendOnlyListEvents.GETALL)
                {
                    a.UpdateClientWithAllData(packet.FromPlayer);
                }
            }
        }
    }
}
