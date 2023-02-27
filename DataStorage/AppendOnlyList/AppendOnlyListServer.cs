using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;

namespace HkmpPouch.DataStorage.AppendOnlyList
{
    public class AppendOnlyListServer
    {
        public string Name;
        public PipeServer pipe;
        private List<ListItem> data = new();

        private bool pendingPrune = false;
        private static Timer EventTimer = new Timer(1000);


        public AppendOnlyListServer(PipeServer pipe, string name)
        {
            this.Name = name;
            this.pipe = pipe;
            AppendOnlyListServer.EventTimer.Elapsed += BatchedPrune;
            AppendOnlyListServer.EventTimer.AutoReset = true;
            AppendOnlyListServer.EventTimer.Enabled = true;
        }

        public void BatchedPrune(System.Object source, ElapsedEventArgs e)
        {
            if (!this.pendingPrune) { return; }
            Prune();
        }

        public void Prune()
        {
            data = data.Where(item => {
                return (DateTime.Now - item.insertedOn).TotalSeconds < item.ttl;
            }).ToList();
            this.pendingPrune = false;
        }

        public void Add(ListItem item)
        {
            this.pendingPrune = true;
            data.Add(item);
            UpdateClientsWithLatestData(item);
        }

        public ListItem LastItem()
        {
            Prune();
            if (data.Count == 0)
            {
                return null;
            }
            return data[data.Count - 1];
        }

        public string SerialiseItems()
        {
            Prune();
            if (data.Count == 0)
            {
                return "";
            }

            return JsonConvert.SerializeObject(data.Select(item => item.value));
        }

        public void UpdateClientWithAllData(ushort toPlayer)
        {
            var serialisedItems = SerialiseItems();

            if (serialisedItems != "")
            {
                pipe.SendToPlayer(toPlayer, AppendOnlyListEvents.GOTALL, $"{this.Name}|{serialisedItems}");
            }
        }
        public void UpdateClientsWithLatestData(ListItem lastItem)
        {
            if (lastItem != null)
            {
                pipe.Broadcast(AppendOnlyListEvents.ADDED, $"{this.Name}|{lastItem.value}");
            }
        }
    }
}
