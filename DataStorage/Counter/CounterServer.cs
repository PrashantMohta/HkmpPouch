using System.Timers;

namespace HkmpPouch.DataStorage.Counter
{
    internal class CounterServer {

        internal string Name;
        internal PipeServer pipe;
        internal int Count { get; private set; }
        private bool pendingUpdates = false;
        private static Timer EventTimer = new Timer(500);
        internal CounterServer(PipeServer pipe, string name)
        {
            this.Count = 0;
            this.Name = name;
            this.pipe = pipe;
            CounterServer.EventTimer.Elapsed += BatchUpdateClients;
            CounterServer.EventTimer.AutoReset = true;
            CounterServer.EventTimer.Enabled = true;
        }

        internal void BatchUpdateClients(System.Object source, ElapsedEventArgs e)
        {
            if (!pendingUpdates) { return; }
            UpdateClients();
            pendingUpdates = false;
        }

        internal void UpdateClients()
        {
            // send updated value to clients
            this.pipe.Broadcast(CounterEvents.UPDATE, $"{this.Name}|{this.Count}");
        }

        internal void UpdateClient(ushort toPlayer)
        {
            this.pipe.SendToPlayer(toPlayer, CounterEvents.UPDATE, $"{this.Name}|{this.Count}");
        }
        internal void Increment(ushort value)
        {
            this.Count += value;
            this.pendingUpdates = true;
        }

        internal void Decrement(ushort value)
        {
            this.Count -= value;
            this.pendingUpdates = true;
        }
    }
}
