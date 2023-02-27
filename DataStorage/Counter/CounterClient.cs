using HkmpPouch.Packets;
using System;

namespace HkmpPouch.DataStorage.Counter
{
    public class CounterClient
    {

        public string Name;
        public int Count { get; private set; }
        public event EventHandler<CounterUpdateEventArgs> OnUpdate;
        private PipeClient pipe;

        public CounterClient(PipeClient pipe, string name)
        {
            this.pipe = pipe;
            this.Name = name;
            this.Count = 0;
            pipe.OnRecieve += HandleEvent;
        }

        private void HandleEvent(object sender, RecievedEventArgs e)
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

        public void Get()
        {
            this.pipe.SendToServer(CounterEvents.GET, this.Name);
        }
        public void Increment(ushort value = 1)
        {
            // send increment command to server
            this.pipe.SendToServer(CounterEvents.INCREMENT, $"{this.Name}|{value}");
        }

        public void Decrement(ushort value = 1)
        {
            // send decrement command to server
            this.pipe.SendToServer(CounterEvents.DECREMENT, $"{this.Name}|{value}");

        }

        public void Destroy()
        {
            pipe.OnRecieve -= HandleEvent;
        }
    }
}
