namespace HkmpPouch.PouchDataServer{

    public class Counter{
        public string Name;
        public string modName;
        public int Count {get; private set;}

        public Counter(string modName, string name){
            this.Count = 0;
            this.Name = name;
            this.modName = modName;
        }

        public void UpdateClients(){
            // send updated value to clients
            Server.Instance.sendToAll(this.modName,CounterEvents.UPDATE,$"{this.Name}|{this.Count}",true);
        }

        public void UpdateClient(ushort toPlayer){
            Server.Instance.send(0,toPlayer,this.modName,CounterEvents.UPDATE,$"{this.Name}|{this.Count}",false,false,true);
        }
        public void Increment(){
            this.Count += 1;
            UpdateClients();
        }

        public void Decrement(){
            this.Count -= 1;
            UpdateClients();
        }
    }
}