using Hkmp.Api.Server;
using HkmpPouch.Packets;
using System;

namespace HkmpPouch
{
    public class PipeServer
    {
        public string ModName { get; }
        public IServerApi ServerApi { get => Server.Instance.Api; }

        public event EventHandler<RecievedEventArgs> OnRecieve;

        public PipeServer(string ModName)
        {
            this.ModName = ModName;
            Server.Instance.OnRecieve += HandleRecieve;
        }

        private void HandleRecieve(object sender, RecievedEventArgs e)
        {
            if (e.Data.ModName != ModName) { return; }
            OnRecieve?.Invoke(this, e);
        }

        public void SendToPlayer(ushort ToPlayer, string EventName, string EventData) {
            Server.Instance.Send<ToPlayerPacket>(PacketsEnum.ToPlayerPacket, new ToPlayerPacket {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                toPlayer = ToPlayer
            }, ToPlayer);
        }
        public void Broadcast(string EventName, string EventData) {
            BroadcastInScene(EventName, EventData, Constants.AllScenes);
        }
        public void BroadcastInScene(string EventName, string EventData, string SceneName) {
            Server.Instance.Broadcast(new ToPlayersPacket
            {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                sceneName = SceneName
            });
        }
    }
}
