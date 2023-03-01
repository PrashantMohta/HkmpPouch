using Hkmp.Api.Server;
using HkmpPouch.Packets;
using System;

namespace HkmpPouch
{
    /// <summary>
    /// A pipe to send and recieve data on the server side.
    /// </summary>
    public class PipeServer
    {
        /// <summary>
        /// Name of the Pipe (typically one mod should only require one pipe)
        /// </summary>
        public string ModName { get; private set; }

        /// <summary>
        /// Instance of Hkmp.Api.Server.IServerApi provided by HKMP
        /// </summary>
        public IServerApi ServerApi { get => Server.Instance.Api; }

        /// <summary>
        /// An event that fires when new data is recieved on the server
        /// </summary>
        public event EventHandler<ReceivedEventArgs> OnRecieve;

        /// <summary>
        /// Create a new PipeServer
        /// </summary>
        /// <param name="ModName">Name of the Mod creating the pipe that is used as a unique identifier for the mod</param>
        public PipeServer(string ModName)
        {
            this.ModName = ModName;
            Server.Instance.OnRecieve += HandleRecieve;
        }

        private void HandleRecieve(object sender, ReceivedEventArgs e)
        {
            if (e.Data.ModName != ModName) { return; }
            OnRecieve?.Invoke(this, e);
        }
        /// <summary>
        /// Send Event to a single Player
        /// </summary>
        /// <param name="ToPlayer">Player id to send the event to</param>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToPlayer(ushort ToPlayer, string EventName, string EventData, bool IsReliable = true) {
            Server.Instance.Send<ToPlayerPacket>(PacketsEnum.ToPlayerPacket, new ToPlayerPacket {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                toPlayer = ToPlayer,
                _isReliable = IsReliable
            }, ToPlayer);
        }
        /// <summary>
        /// Send Event to all the connected Players
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void Broadcast(string EventName, string EventData, bool IsReliable = true) {
            BroadcastInScene(EventName, EventData, Constants.AllScenes, IsReliable);
        }
        /// <summary>
        /// Send Event to all the connected Players in a particular scene
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="SceneName">Name of the scene to send the data in</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void BroadcastInScene(string EventName, string EventData, string SceneName, bool IsReliable = true) {
            Server.Instance.Broadcast(new ToPlayersPacket
            {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                sceneName = SceneName,
                _isReliable = IsReliable
            });
        }
    }
}
