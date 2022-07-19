using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Timers;

namespace HkmpPouch.PouchDataServer{

    public class AppendOnlyList{
        public string Name;
        public string modName;
        private List<ListItem> data = new();

        private bool pendingPrune = false;
        private static Timer EventTimer = new Timer(1000);


        public AppendOnlyList(string modName, string name){
            this.Name = name;
            this.modName = modName;
            AppendOnlyList.EventTimer.Elapsed += BatchedPrune;
            AppendOnlyList.EventTimer.AutoReset = true;
            AppendOnlyList.EventTimer.Enabled = true;
        }

        public void BatchedPrune(System.Object source, ElapsedEventArgs e){
            if(!this.pendingPrune) {return;}
            Prune();
        }

        public void Prune(){
            data = data.Where(item => {
                    return (DateTime.Now - item.insertedOn).TotalSeconds < item.ttl;
                }).ToList();
            this.pendingPrune = false;
        }

        public void Add(ListItem item){
            this.pendingPrune = true;
            data.Add(item);
            UpdateClientsWithLatestData(item);
        }

        public ListItem LastItem(){
            Prune();
            if(data.Count == 0){
                return null;
            }
            return data[data.Count - 1];
        }

        public string SerialiseItems(){
            Prune();
            if(data.Count == 0){
                return "";
            }

            return JsonConvert.SerializeObject(data.Select( item => item.value));
        }

        public void UpdateClientWithAllData(ushort toPlayer){
            var serialisedItems = SerialiseItems();
            
            if(serialisedItems != ""){
                Server.Instance.Send(0,toPlayer,this.modName,AppendOnlyListEvents.GOTALL,$"{this.Name}|{serialisedItems}",false,false,true);
            } else {
                Platform.LogDebug($"{modName} List {Name} Has no items to send");
            }
        }
        public void UpdateClientsWithLatestData(ListItem lastItem){
            if(lastItem != null){
                Server.Instance.SendToAll(this.modName,AppendOnlyListEvents.ADDED,$"{this.Name}|{lastItem.value}",true);
            }
        }
    }
}