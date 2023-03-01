using HkmpPouch.Packets;
using System;

namespace HkmpPouch.DataStorage.Counter
{
    /// <summary>
    /// The CounterClient class
    /// </summary>
    public class CounterClient
    {

        /// <summary>
        /// Name of the counter
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Last updated Counter value
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Event fired when Counter is updated
        /// </summary>
        public event EventHandler<CounterUpdateEventArgs> OnUpdate;

        private PipeClient pipe;

        /// <summary>
        /// Creates a Server-backed shared counter
        /// </summary>
        /// <param name="pipe"> A PipeClient to use to maintain the counter</param>
        /// <param name="name"> Name of the counter </param>
        public CounterClient(PipeClient pipe, string name)
        {
            this.pipe = pipe;
            this.Name = name;
            this.Count = 0;
            pipe.OnRecieve += HandleEvent;
        }

        private void HandleEvent(object sender, ReceivedEventArgs e)
        {
            var packet = e.Data;
            if (packet.EventName == CounterEvents.UPDATE)
            {
                var data = packet.EventData.Split('|');
                if (Name == data[0])
                {
                    this.Count = Int32.Parse(data[1]);
                    OnUpdate?.Invoke(this, new CounterUpdateEventArgs
                    {
                        Count = this.Count
                    });
                }
            }
        }

        /// <summary>
        /// Get the currently latest value from the server
        /// </summary>
        public void Get()
        {
            this.pipe.SendToServer(CounterEvents.GET, this.Name);
        }
        /// <summary>
        /// Increment the counter value
        /// </summary>
        /// <param name="value">The value to increment by</param>
        public void Increment(ushort value = 1)
        {
            // send increment command to server
            this.pipe.SendToServer(CounterEvents.INCREMENT, $"{this.Name}|{value}");
        }
        /// <summary>
        /// Decrement the counter value
        /// </summary>
        /// <param name="value">The value to decrement by</param>
        public void Decrement(ushort value = 1)
        {
            // send decrement command to server
            this.pipe.SendToServer(CounterEvents.DECREMENT, $"{this.Name}|{value}");

        }

        /// <summary>
        /// Destroy the CounterClient safely
        /// </summary>
        public void Destroy()
        {
            pipe.OnRecieve -= HandleEvent;
        }
    }
}
