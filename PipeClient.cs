using Hkmp.Api.Client;
using HkmpPouch.Networking;
using HkmpPouch.Packets;
using System;

namespace HkmpPouch
{

    /// <summary>
    /// A pipe to send and recieve data on the client side.
    /// </summary>
    public class PipeClient : OnAble
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
        /// An event that fires when ClientApi is available
        /// </summary>
        public event EventHandler<EventArgs> OnReady;


        /// <summary>
        /// An event that fires when new data is recieved on the client
        /// </summary>
        public event EventHandler<ReceivedEventArgs> OnRecieve;

        /// <summary>
        /// The Logger to use
        /// </summary>
        public Logger Logger { get; private set; }

        private bool IsListening = false;

        /// <summary>
        /// Create a new PipeClient
        /// </summary>
        /// <param name="ModName">Name of the Mod creating the pipe that is used as a unique identifier for the mod</param>
        public PipeClient(string ModName)
        {
            this.ModName = ModName;
            Client.PipeClients.Add(this);
            Client.OnReady += Client_OnReady;
        }

        private void ClientManager_ConnectEvent()
        {
            Client.Instance.RegisterPipeWithServer(this);
        }

        private void Client_OnReady(object sender, EventArgs e)
        {
            if (IsListening) { return; }
            this.Logger = new Logger(this.ModName, Client.Instance);
            ClientApi.ClientManager.ConnectEvent += ClientManager_ConnectEvent;
            Client.Instance.RegisterPipeWithServer(this);
            Logger.Info("Client Ready");
            OnReady?.Invoke(this, new EventArgs());
            Client.Instance.OnRecieve += HandleRecieve;
            IsListening = true;
        }

        private void HandleRecieve(object sender, ReceivedEventArgs e)
        {
            if (e.Data.ModName != ModName) { return; }
            Logger.Debug($"Client Received event {e.Data.EventName} = {e.Data.EventData}");
            OnRecieve?.Invoke(this, e);
            base.TriggerEvents(e.Data.EventName, e.Data);
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
            SendToPlayer(PlayerId, EventName, EventData, null, SameScene, IsReliable);
        }

        /// <summary>
        /// Send an event to a single player
        /// </summary>
        /// <param name="PlayerId">Player Id of the recieving player</param>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="ExtraBytes">Extra byte[] to send with the event</param>
        /// <param name="SameScene">Should the receiving player be in the same scene</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToPlayer(ushort PlayerId, string EventName, string EventData, byte[] ExtraBytes = null, bool SameScene = true, bool IsReliable = true)
        {
            Logger.Debug($"Client SendToPlayer {PlayerId} event {EventName} = {EventData}");
            Client.Instance.Send<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, new PlayerToPlayerPacket
            {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                extraBytes = ExtraBytes,
                toPlayer = PlayerId,
                sceneName = SameScene ? Constants.SameScenes : Constants.AllScenes,
                _isReliable = IsReliable
            });
        }

        /// <summary>
        /// Send an event to a single player
        /// </summary>
        /// <param name="PlayerId">Player Id of the recieving player</param>
        /// <param name="pipeEvent">The pipeEvent to send</param>
        /// <param name="SameScene">Should the receiving player be in the same scene</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToPlayer(ushort PlayerId, PipeEvent pipeEvent, bool SameScene = true, bool IsReliable = true)
        {
            SendToPlayer(PlayerId, pipeEvent.GetName(), pipeEvent.ToString(), pipeEvent.ExtraBytes, SameScene, IsReliable);
        }


        /// <summary>
        /// Only send event to the server (presumably to be handled by a server side addon)
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="ExtraBytes">Extra byte[] to send with the event</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToServer(string EventName, string EventData,byte[] ExtraBytes = null, bool IsReliable = true)
        {
            Logger.Debug($"Client SendToServer event {EventName} = {EventData}");
            Client.Instance.Send<ToServerPacket>(PacketsEnum.ToServerPacket, new ToServerPacket
            {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                extraBytes = ExtraBytes,
                _isReliable = IsReliable
            });
        }

        /// <summary>
        /// Only send event to the server (presumably to be handled by a server side addon)
        /// </summary>
        /// <param name="pipeEvent">The pipeEvent to send</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToServer(PipeEvent pipeEvent, bool IsReliable = true)
        {
            SendToServer(pipeEvent.GetName(), pipeEvent.ToString(), pipeEvent.ExtraBytes, IsReliable);
        }

        /// <summary>
        /// Send an event to many players
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="SameScene">Should the receiving player be in the same scene</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void Broadcast(string EventName, string EventData, bool SameScene = true, bool IsReliable = true)
        {
            Broadcast(EventName, EventData, null, SameScene, IsReliable);
        }

        /// <summary>
        /// Send an event to many players
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="ExtraBytes">Extra byte[] to send with the event</param>
        /// <param name="SameScene">Should the receiving player be in the same scene</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void Broadcast(string EventName, string EventData, byte[] ExtraBytes = null, bool SameScene = true, bool IsReliable = true)
        {
            if (SameScene)
            {
                BroadcastInScene(EventName, EventData, Constants.SameScenes, ExtraBytes, IsReliable);
            }
            else
            {
                BroadcastInScene(EventName, EventData, Constants.AllScenes, ExtraBytes, IsReliable);
            }
        }


        /// <summary>
        /// Send an event to many players
        /// </summary>
        /// <param name="pipeEvent">The pipeEvent to send</param>
        /// <param name="SameScene">Should the receiving player be in the same scene</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void Broadcast(PipeEvent pipeEvent, bool SameScene = true, bool IsReliable = true)
        {
            if (SameScene)
            {
                BroadcastInScene(pipeEvent, Constants.SameScenes, IsReliable);
            }
            else
            {
                BroadcastInScene(pipeEvent, Constants.AllScenes, IsReliable);
            }
        }
        /// <summary>
        /// Send Event to all the connected Players in a particular scene
        /// </summary>
        /// <param name="pipeEvent">The pipeEvent to send</param>
        /// <param name="SceneName">Name of the scene to send the data in</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void BroadcastInScene(PipeEvent pipeEvent, string SceneName, bool IsReliable = true)
        {

            BroadcastInScene(pipeEvent.GetName(),pipeEvent.ToString(), SceneName, pipeEvent.ExtraBytes, IsReliable);
        }

        /// <summary>
        /// Send Event to all the connected Players in a particular scene
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="SceneName">Name of the scene to send the data in</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void BroadcastInScene(string EventName, string EventData, string SceneName,bool IsReliable = true)
        {
            BroadcastInScene(EventName, EventData, SceneName, null, IsReliable);
        }

        /// <summary>
        /// Send Event to all the connected Players in a particular scene
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="ExtraBytes">Extra byte[] to send with the event</param>
        /// <param name="SceneName">Name of the scene to send the data in</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void BroadcastInScene(string EventName, string EventData, string SceneName, byte[] ExtraBytes = null, bool IsReliable = true)
        {

            Logger.Debug($"Client BroadcastInScene {SceneName} event {EventName} = {EventData}");
            Client.Instance.Send<PlayerToPlayersPacket>(PacketsEnum.PlayerToPlayersPacket, new PlayerToPlayersPacket
            {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                extraBytes = ExtraBytes,
                sceneName = SceneName,
                _isReliable = IsReliable
            });
        }

        /// <summary>
        /// Check if this Pipe has a server counter part installed on this server
        /// </summary>
        /// <param name="callback"> called with the boolean result</param>
        public void ServerCounterPartAvailable(Action<bool> callback)
        {
            Client.Instance.HasServerCounterPart(ModName, callback);
        }

        /// <summary>
        /// Destroy the pipe safely
        /// </summary>
        new public void Destroy()
        {
            base.Destroy();
            Client.Instance.OnRecieve -= HandleRecieve;
            OnRecieve = null;
        }
    }
}
