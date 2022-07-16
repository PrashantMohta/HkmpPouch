using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HkmpPouch.PouchDataServer{

    public class AppendOnlyList{
        public string Name;
        public string modName;
        private List<ListItem> data = new();

        public AppendOnlyList(string modName, string name){
            this.Name = name;
            this.modName = modName;
        }

        public void Prune(){
            data = data.Where(item => {
                Modding.Logger.Log($"{item.value} | {item.ttl} | {(DateTime.Now - item.insertedOn).TotalSeconds}");
                return (DateTime.Now - item.insertedOn).TotalSeconds < item.ttl;
                }).ToList();
        }

        public void Add(ListItem item){
            Prune();
            data.Add(item);
            UpdateClientsWithLatestData();
        }

        public List<ListItem> Items(){
            Prune();
            return data;
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
             Modding.Logger.Log("list json" + JsonConvert.SerializeObject(data.Select( item => item.value)));

            return JsonConvert.SerializeObject(data.Select( item => item.value));
        }

        public void UpdateClientWithAllData(ushort toPlayer){
            var serialisedItems = SerialiseItems();
            
            if(serialisedItems != ""){
                Server.Instance.send(0,toPlayer,this.modName,AppendOnlyListEvents.GOTALL,$"{this.Name}|{serialisedItems}",false,false,true);
            } else {
                Modding.Logger.Log("no items to SEND");
            }
        }
        public void UpdateClientsWithLatestData(){
            var lastItem = LastItem();
            if(lastItem != null){
                Server.Instance.sendToAll(this.modName,AppendOnlyListEvents.ADDED,$"{this.Name}|{lastItem.value}",true);
            }
        }
    }
}