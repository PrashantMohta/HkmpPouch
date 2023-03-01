using HkmpPouch.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HkmpPouch.DataStorage.AppendOnlyList
{
    /// <summary>
    /// The AppendOnlyListClient class
    /// </summary>
    public class AppendOnlyListClient
    {
        /// <summary>
        /// The Name of the AppendOnlyList
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The current list data
        /// </summary>
        public List<string> ListData { get; private set; } = new();


        /// <summary>
        /// Event fired when AppendOnlyList is updated
        /// </summary>
        public event EventHandler<AppendOnlyListUpdateEventArgs> OnUpdate;

        private PipeClient pipe;

        /// <summary>
        /// Creates a Server-backed shared list of strings that can only be appended to
        /// </summary>
        /// <param name="pipe"> A PipeClient to use to maintain the counter</param>
        /// <param name="name"> Name of the AppendOnlyList </param>
        public AppendOnlyListClient(PipeClient pipe, string name)
        {
            this.pipe = pipe;
            this.Name = name;
            pipe.OnRecieve += HandleEvent;
        }

        private void HandleEvent(System.Object _, ReceivedEventArgs args)
        {
            var packet = args.Data;
            if (!packet.EventData.StartsWith($"{Name}|")) { return; }
            if (packet.EventName == AppendOnlyListEvents.ADDED)
            {
                var data = packet.EventData.Split(new Char[] { '|' }, 2);
                var item = data[1];
                ListData.Add(item);
                OnUpdate?.Invoke(this, new AppendOnlyListUpdateEventArgs
                {
                    data = this.ListData
                });

            }
            if (packet.EventName == AppendOnlyListEvents.GOTALL)
            {
                var data = packet.EventData.Split(new Char[] { '|' }, 2);
                ListData = JsonConvert.DeserializeObject<List<string>>(data[1]);
                OnUpdate?.Invoke(this, new AppendOnlyListUpdateEventArgs
                {
                    data = this.ListData
                });
            }
        }

        /// <summary>
        /// Add an Item to the list
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="ttl">The number of seconds the item should be preserved on the server</param>
        public void Add(string item, int ttl)
        {
            // send add command to server
            this.pipe.SendToServer(AppendOnlyListEvents.ADD, $"{this.Name}|{ttl}|{item}");
        }
        /// <summary>
        /// Get the entire AppendOnlyList
        /// </summary>
        public void GetAll()
        {
            // send getAll command to server
            this.pipe.SendToServer(AppendOnlyListEvents.GETALL, this.Name);
        }
        /// <summary>
        /// Destroy the AppendOnlyListClient safely
        /// </summary>
        public void Destory()
        {
            pipe.OnRecieve -= HandleEvent;
        }
    }
}
