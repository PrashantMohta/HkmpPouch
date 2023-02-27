using System.Timers;

namespace HkmpPouch.DataStorage.Counter
{
    public class CounterServer { 

        public string Name;
        public PipeServer pipe;
        public int Count { get; private set; }
        private bool pendingUpdates = false;
        private static Timer EventTimer = new Timer(500);
        public CounterServer(PipeServer pipe, string name)
        {
            this.Count = 0;
            this.Name = name;
            this.pipe = pipe;
            CounterServer.EventTimer.Elapsed += BatchUpdateClients;
            CounterServer.EventTimer.AutoReset = true;
            CounterServer.EventTimer.Enabled = true;
        }

        public void BatchUpdateClients(System.Object source, ElapsedEventArgs e)
        {
            if (!pendingUpdates) { return; }
            UpdateClients();
            pendingUpdates = false;
        }

        public void UpdateClients()
        {
            // send updated value to clients
            this.pipe.Broadcast(CounterEvents.UPDATE, $"{this.Name}|{this.Count}");

        }

        public void UpdateClient(ushort toPlayer)
        {
            this.pipe.SendToPlayer(toPlayer, CounterEvents.UPDATE, $"{this.Name}|{this.Count}");
        }
        public void Increment(ushort value)
        {
            this.Count += value;
            this.pendingUpdates = true;
        }

        public void Decrement(ushort value)
        {
            this.Count -= value;
            this.pendingUpdates = true;
        }
    }
}
