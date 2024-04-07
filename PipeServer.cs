using Hkmp.Api.Server;
using HkmpPouch.Networking;
using HkmpPouch.Packets;
using System;

namespace HkmpPouch
{
    /// <summary>
    /// A pipe to send and recieve data on the server side.
    /// </summary>
    public class PipeServer : OnAble
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
        /// The Logger to use
        /// </summary>
        public Logger Logger { get; private set; }

        /// <summary>
        /// Create a new PipeServer
        /// </summary>
        /// <param name="ModName">Name of the Mod creating the pipe that is used as a unique identifier for the mod</param>
        public PipeServer(string ModName)
        {
            this.ModName = ModName;
            this.Logger = new Logger(this.ModName, Server.Instance);
            Server.Instance.AddPipe(this.ModName);
            Server.Instance.OnRecieve += HandleRecieve;
            Logger.Info("PipeServer Created");
        }

        private void HandleRecieve(object sender, ReceivedEventArgs e)
        {
            if (e.Data.ModName != ModName) { return; }
            Logger.Debug($"Server Received event {e.Data.EventName} = {e.Data.EventData}");
            OnRecieve?.Invoke(this, e);
            base.TriggerEvents(e.Data.EventName, e.Data);
        }


        /// <summary>
        /// Send Event to a single Player
        /// </summary>
        /// <param name="ToPlayer">Player id to send the event to</param>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="ExtraBytes">Extra byte[] to send with the event</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToPlayer(ushort ToPlayer, string EventName, string EventData,byte[] ExtraBytes = null, bool IsReliable = true)
        {

            Logger.Debug($"Server SendToPlayer {ToPlayer} event {EventName} = {EventData}");
            Server.Instance.Send<ToPlayerPacket>(PacketsEnum.ToPlayerPacket, new ToPlayerPacket
            {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                extraBytes = ExtraBytes,
                toPlayer = ToPlayer,
                _isReliable = IsReliable
            }, ToPlayer);
        }

        /// <summary>
        /// Send Event to a single Player
        /// </summary>
        /// <param name="ToPlayer">Player id to send the event to</param>
        /// <param name="pipeEvent">The pipeEvent to send</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void SendToPlayer(ushort ToPlayer, PipeEvent pipeEvent, bool IsReliable = true)
        {
            SendToPlayer(ToPlayer, pipeEvent.GetName(),pipeEvent.ToString(), pipeEvent.ExtraBytes, IsReliable);
        }
        /// <summary>
        /// Send Event to all the connected Players
        /// </summary>
        /// <param name="pipeEvent">The pipeEvent to send</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void Broadcast(PipeEvent pipeEvent, bool IsReliable = true)
        {
            Broadcast(pipeEvent.GetName(), pipeEvent.ToString(), pipeEvent.ExtraBytes, IsReliable);
        }

        /// <summary>
        /// Send Event to all the connected Players
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="ExtraBytes">Extra byte[] to send with the event</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void Broadcast(string EventName, string EventData, byte[] ExtraBytes = null, bool IsReliable = true)
        {
            BroadcastInScene(EventName, EventData, Constants.AllScenes, ExtraBytes, IsReliable);
        }
        /// <summary>
        /// Send Event to all the connected Players in a particular scene
        /// </summary>
        /// <param name="EventName">Name of your custom event</param>
        /// <param name="EventData">Corresponding event data</param>
        /// <param name="ExtraBytes">Extra byte[] to send with the event</param>
        /// <param name="SceneName">Name of the scene to send the data in</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void BroadcastInScene(string EventName, string EventData, string SceneName, byte[] ExtraBytes = null,  bool IsReliable = true)
        {
            Logger.Debug($"Server BroadcastInScene {SceneName} event {EventName} = {EventData}");
            Server.Instance.Broadcast(new ToPlayersPacket
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
        /// Send Event to all the connected Players in a particular scene
        /// </summary>
        /// <param name="pipeEvent">The pipeEvent to send</param>
        /// <param name="SceneName">Name of the scene to send the data in</param>
        /// <param name="IsReliable">Should the packed be resent if undelivered</param>
        public void BroadcastInScene(PipeEvent pipeEvent, string SceneName, bool IsReliable = true)
        {
            BroadcastInScene(pipeEvent.GetName(), pipeEvent.ToString(), SceneName, pipeEvent.ExtraBytes, IsReliable);
        }


        /// <summary>
        /// Destroy the pipe safely
        /// </summary>
        new public void Destroy()
        {
            base.Destroy();
            Server.Instance.OnRecieve -= HandleRecieve;
            OnRecieve = null;
        }
    }
}
