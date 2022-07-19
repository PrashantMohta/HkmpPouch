using System.Timers;
namespace HkmpPouch.PouchDataServer{

    public class Counter{
        public string Name;
        public string modName;
        public int Count {get; private set;}
        private bool pendingUpdates = false;
        private static Timer EventTimer = new Timer(500);
        public Counter(string modName, string name){
            this.Count = 0;
            this.Name = name;
            this.modName = modName;
            Counter.EventTimer.Elapsed += BatchUpdateClients;
            Counter.EventTimer.AutoReset = true;
            Counter.EventTimer.Enabled = true;
        }

        public void BatchUpdateClients(System.Object source, ElapsedEventArgs e){
            if(!pendingUpdates){ return;}
            UpdateClients();
            pendingUpdates = false;
        }

        public void UpdateClients(){
            // send updated value to clients
            Server.Instance.SendToAll(this.modName,CounterEvents.UPDATE,$"{this.Name}|{this.Count}",true);
        }

        public void UpdateClient(ushort toPlayer){
            Server.Instance.Send(0,toPlayer,this.modName,CounterEvents.UPDATE,$"{this.Name}|{this.Count}",false,false,true);
        }
        public void Increment(ushort value){
            this.Count += value;
            this.pendingUpdates = true;
        }

        public void Decrement(ushort value){
            this.Count -= value;
            this.pendingUpdates = true;
        }
    }
}