using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace HkmpPouch.DataStorage.AppendOnlyList
{
    internal class AppendOnlyListServer
    {
        internal string Name;
        internal PipeServer pipe;
        internal List<ListItem> data = new();

        private bool pendingPrune = false;
        private static Timer EventTimer = new Timer(1000);


        internal AppendOnlyListServer(PipeServer pipe, string name)
        {
            this.Name = name;
            this.pipe = pipe;
            AppendOnlyListServer.EventTimer.Elapsed += BatchedPrune;
            AppendOnlyListServer.EventTimer.AutoReset = true;
            AppendOnlyListServer.EventTimer.Enabled = true;
        }

        internal void BatchedPrune(System.Object source, ElapsedEventArgs e)
        {
            if (!this.pendingPrune) { return; }
            Prune();
        }

        internal void Prune()
        {
            data = data.Where(item => {
                return (DateTime.Now - item.insertedOn).TotalSeconds < item.ttl;
            }).ToList();
            this.pendingPrune = false;
        }

        internal void Add(ListItem item)
        {
            this.pendingPrune = true;
            data.Add(item);
            UpdateClientsWithLatestData(item);
        }

        internal ListItem LastItem()
        {
            Prune();
            if (data.Count == 0)
            {
                return null;
            }
            return data[data.Count - 1];
        }

        internal string SerialiseItems()
        {
            Prune();
            if (data.Count == 0)
            {
                return "";
            }

            return JsonConvert.SerializeObject(data.Select(item => item.value));
        }

        internal void UpdateClientWithAllData(ushort toPlayer)
        {
            var serialisedItems = SerialiseItems();

            if (serialisedItems != "")
            {
                pipe.SendToPlayer(toPlayer, AppendOnlyListEvents.GOTALL, $"{this.Name}|{serialisedItems}");
            }
        }
        internal void UpdateClientsWithLatestData(ListItem lastItem)
        {
            if (lastItem != null)
            {
                pipe.Broadcast(AppendOnlyListEvents.ADDED, $"{this.Name}|{lastItem.value}");
            }
        }
    }
}
