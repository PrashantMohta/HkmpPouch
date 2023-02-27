using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;
using HkmpPouch.Packets;
using System;

namespace HkmpPouch
{
    internal class Client : ClientAddon
    {
        public override bool NeedsNetwork => true;

        protected override string Name => Constants.Name;

        protected override string Version => Constants.Version;

        internal static Client Instance;

        internal IClientApi Api;

        internal static event EventHandler<EventArgs> OnReady;

        internal event EventHandler<RecievedEventArgs> OnRecieve;

        internal IClientAddonNetworkReceiver<PacketsEnum> NetReceiver;
        internal IClientAddonNetworkSender<PacketsEnum> NetSender;
        public Client() {
            Instance = this;
        }
        internal void Error(string str)
        {
            Logger.Error(str);
        }
        internal void Info(string str)
        {
            Logger.Info(str);
        }

        internal void RecieveData(RecievedData r)
        {
            OnRecieve?.Invoke(this, new RecievedEventArgs { Data = r });
        }

        public override void Initialize(IClientApi clientApi)
        {
            Api = clientApi;
            NetReceiver = Api.NetClient.GetNetworkReceiver<PacketsEnum>(Instance, PacketBoi.InstantiatePacket);
            NetReceiver.RegisterPacketHandler<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, PlayerToPlayerPacketHandler);
            NetReceiver.RegisterPacketHandler<PlayerToPlayersPacket>(PacketsEnum.PlayerToPlayersPacket, PlayerToPlayersPacketHandler);
            NetReceiver.RegisterPacketHandler<ToPlayerPacket>(PacketsEnum.ToPlayerPacket, ToPlayerPacketHandler);
            NetReceiver.RegisterPacketHandler<ToPlayersPacket>(PacketsEnum.ToPlayersPacket, ToPlayersPacket);
            OnReady?.Invoke(this, EventArgs.Empty);
        }

        internal void Send<TPacketData>(PacketsEnum PacketId,TPacketData PacketData) where TPacketData : IPacketData, new() {
            if (!Api.NetClient.IsConnected)
            {
                return;
            }
            if (NetSender == null)
            {
                NetSender = Api.NetClient.GetNetworkSender<PacketsEnum>(Instance);
            }
            NetSender.SendCollectionData(PacketId, PacketData);
        }


        internal void PlayerToPlayerPacketHandler(PlayerToPlayerPacket packet)
        {
            RecieveData(new RecievedData
            {
                IsReliable = packet.IsReliable,
                DropReliableDataIfNewerExists = packet.DropReliableDataIfNewerExists,
                ToPlayer = packet.toPlayer,
                FromPlayer = packet.fromPlayer,
                SceneName = packet.sceneName,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
            });
        }

        internal  void PlayerToPlayersPacketHandler(PlayerToPlayersPacket packet)
        {
            RecieveData(new RecievedData
            {
                IsReliable = packet.IsReliable,
                DropReliableDataIfNewerExists = packet.DropReliableDataIfNewerExists,
                SceneName = packet.sceneName,
                FromPlayer = packet.fromPlayer,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
            });
        }

        internal  void ToPlayerPacketHandler(ToPlayerPacket packet)
        {
            RecieveData(new RecievedData
            {
                IsReliable = packet.IsReliable,
                DropReliableDataIfNewerExists = packet.DropReliableDataIfNewerExists,
                ToPlayer = packet.toPlayer,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
            });
        }

        internal  void ToPlayersPacket(ToPlayersPacket packet)
        {
            RecieveData(new RecievedData
            {
                IsReliable = packet.IsReliable,
                DropReliableDataIfNewerExists = packet.DropReliableDataIfNewerExists,
                SceneName = packet.sceneName,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
            });
        }



    }
}