using Hkmp.Api.Client;
using HkmpPouch.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HkmpPouch
{
    public class PipeClient
    {
        public string ModName { get; }
        public IClientApi ClientApi { get => Client.Instance.Api; }

        public event EventHandler<RecievedEventArgs> OnRecieve;
        private bool IsListening = false;
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

        private void HandleRecieve(object sender, RecievedEventArgs e)
        {
            if (e.Data.ModName != ModName) { return; }
            OnRecieve?.Invoke(this, e);
        }

        public void SendToServer(string EventName,string EventData) {
            Client.Instance.Send<ToServerPacket>(PacketsEnum.ToServerPacket, new ToServerPacket { mod = ModName, eventName = EventName,eventData = EventData});
        }
        public void SendToPlayer(ushort PlayerId, string EventName, string EventData, bool SameScene = true) {
            Client.Instance.Send<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, new PlayerToPlayerPacket { 
                mod = ModName, 
                eventName = EventName, 
                eventData = EventData,
                toPlayer = PlayerId,
                sceneName = SameScene ? Constants.SameScenes : Constants.AllScenes
            });
        }
        public void Broadcast(string EventName, string EventData, bool SameScene = true) {
            if (SameScene)
            {
                BroadcastInScene(EventName, EventData, Constants.SameScenes);
            }
            else 
            {
                BroadcastInScene(EventName, EventData, Constants.AllScenes);
            }
        }
        public void BroadcastInScene(string EventName, string EventData, string SceneName) {
            Client.Instance.Send<PlayerToPlayersPacket>(PacketsEnum.PlayerToPlayersPacket, new PlayerToPlayersPacket {
                mod = ModName,
                eventName = EventName,
                eventData = EventData,
                sceneName = SceneName
            });
        }
    }
}
