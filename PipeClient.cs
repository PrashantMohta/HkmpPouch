using Hkmp.Api.Client;
using HkmpPouch.Packets;
using System;

namespace HkmpPouch
{
    
    /// <summary>
    /// A pipe to send and recieve data on the client side.
    /// </summary>
    public class PipeClient
    {
        /// <summary>
        /// Name of the Pipe (typically one mod should only require one pipe)
        /// </summary>
        public string ModName { get; private set; }

        /// <summary>
        /// Instance of Hkmp.Api.Client.IClientApi provided by HKMP
        /// </summary>
        public IClientApi ClientApi { get => Client.Instance.Api; }


        /// <summary>
        /// An event that fires when new data is recieved on the client
        /// </summary>
        public event EventHandler<ReceivedEventArgs> OnRecieve;

        private bool IsListening = false;

        /// <summary>
        /// Create a new PipeClient
        /// </summary>
        /// <param name="ModName">Name of the Mod creating the pipe that is used as a unique identifier for the mod</param>
        public PipeClient(string ModName)
        {
            this.ModName = ModName;
            Client.OnReady += Client_OnReady;
        }

        private void Client_OnReady(object sender, EventArgs e)
        {
            if (IsListening) { return; }
            Client.Instance.OnRecieve += HandleRecieve;
            IsListening = true;
        }

        private void HandleRecieve(object sender, ReceivedEventArgs e)
        {
            if (e.Data.ModName != ModName) { return; }
            OnRecieve?.Invoke(this, e);
        }

        /// <summary>
        /// Only send event to the server (presumably to be handled by a server side addon)
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToServer(string EventName,string EventData,bool IsReliable = true) {
            Client.Instance.Send<ToServerPacket>(PacketsEnum.ToServerPacket, new ToServerPacket {
                mod = ModName, 
                eventName = EventName,
                eventData = EventData,
                _isReliable = IsReliable
            });
        }
        /// <summary>
        /// Send an event to a single player
        /// </summary>
        /// <param name="PlayerId">Player Id of the recieving player</param>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="SameScene">Should the receiving player be in the same scene</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToPlayer(ushort PlayerId, string EventName, string EventData, bool SameScene = true, bool IsReliable = true)
        {
            Client.Instance.Send<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, new PlayerToPlayerPacket
            {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                toPlayer = PlayerId,
                sceneName = SameScene ? Constants.SameScenes : Constants.AllScenes,
                _isReliable = IsReliable
            });
        }

        /// <summary>
        /// Send an event to many players
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="SameScene">Should the receiving player be in the same scene</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void Broadcast(string EventName, string EventData, bool SameScene = true, bool IsReliable = true) {
            if (SameScene)
            {
                BroadcastInScene(EventName, EventData, Constants.SameScenes, IsReliable);
            }
            else 
            {
                BroadcastInScene(EventName, EventData, Constants.AllScenes, IsReliable);
            }
        }
        /// <summary>
        /// Send Event to all the connected Players in a particular scene
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="SceneName">Name of the scene to send the data in</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void BroadcastInScene(string EventName, string EventData, string SceneName, bool IsReliable = true) {
            Client.Instance.Send<PlayerToPlayersPacket>(PacketsEnum.PlayerToPlayersPacket, new PlayerToPlayersPacket {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                sceneName = SceneName,
                _isReliable = IsReliable
            });
        }
    }
}
