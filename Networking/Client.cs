using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;
using HkmpPouch.Packets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HkmpPouch.Networking
{
    internal class Client : ClientAddon, ILogger
    {
        public override bool NeedsNetwork => true;

        protected override string Name => Constants.Name;

        protected override string Version => Constants.Version;

        internal static Client Instance;

        internal IClientApi Api;

        internal static event EventHandler<EventArgs> OnReady;

        internal static event EventHandler<EventArgs> OnMetadataUpdate;

        internal event EventHandler<ReceivedEventArgs> OnRecieve;

        internal IClientAddonNetworkReceiver<PacketsEnum> NetReceiver;
        internal IClientAddonNetworkSender<PacketsEnum> NetSender;
        internal bool HasServerPipeList { get; private set; } = false;
        private List<string> ServerPipeList = new();
        public Client() {
            Instance = this;
        }
        public void Error(string str)
        {
            Logger.Error(str);
        }
        public void Info(string str)
        {
            Logger.Info(str);
        }
        public void Debug(string str)
        {
            Logger.Debug(str);
        }

        internal void RecieveData(EventContainer r)
        {
            OnRecieve?.Invoke(this, new ReceivedEventArgs { Data = r });
        }

        public override void Initialize(IClientApi clientApi)
        {
            Api = clientApi;
            NetReceiver = Api.NetClient.GetNetworkReceiver<PacketsEnum>(Instance, PacketBoi.InstantiatePacket);
            NetReceiver.RegisterPacketHandler<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, PlayerToPlayerPacketHandler);
            NetReceiver.RegisterPacketHandler<PlayerToPlayersPacket>(PacketsEnum.PlayerToPlayersPacket, PlayerToPlayersPacketHandler);
            NetReceiver.RegisterPacketHandler<ToPlayerPacket>(PacketsEnum.ToPlayerPacket, ToPlayerPacketHandler);
            NetReceiver.RegisterPacketHandler<ToPlayersPacket>(PacketsEnum.ToPlayersPacket, ToPlayersPacket);
            NetReceiver.RegisterPacketHandler<ServerPipeListPacket>(PacketsEnum.ServerPipeListPacket, ServerPipeListPacketHandler);

            OnReady?.Invoke(this, EventArgs.Empty);
            Api.ClientManager.ConnectEvent += ClientManager_ConnectEvent;
            Api.ClientManager.DisconnectEvent += ClientManager_DisconnectEvent;
        }

        private void ServerPipeListPacketHandler(ServerPipeListPacket packet)
        {
            HasServerPipeList = true;
            ServerPipeList = packet.PipeList;
            OnMetadataUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void ClientManager_DisconnectEvent()
        {
            HasServerPipeList = false;
            ServerPipeList.Clear();
            OnMetadataUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void ClientManager_ConnectEvent()
        {
            Logger.Info("sending get packet");
            Send<GetServerMetadataPacket>(PacketsEnum.GetServerMetadataPacket, new GetServerMetadataPacket
            {
                MetadataKey = (byte)METADATA.INSTALLED_PIPES
            });
        }

        internal void Send<TPacketData>(PacketsEnum PacketId, TPacketData PacketData) where TPacketData : IPacketData, new() {
            if (Api == null || !Api.NetClient.IsConnected)
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
            RecieveData(new EventContainer
            {
                IsReliable = packet.IsReliable,
                ToPlayer = packet.toPlayer,
                FromPlayer = packet.fromPlayer,
                SceneName = packet.sceneName,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
                ExtraBytes = packet.extraBytes,
            });
        }

        internal void PlayerToPlayersPacketHandler(PlayerToPlayersPacket packet)
        {
            RecieveData(new EventContainer
            {
                IsReliable = packet.IsReliable,
                SceneName = packet.sceneName,
                FromPlayer = packet.fromPlayer,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
                ExtraBytes = packet.extraBytes,
            });
        }

        internal void ToPlayerPacketHandler(ToPlayerPacket packet)
        {
            RecieveData(new EventContainer
            {
                IsReliable = packet.IsReliable,
                ToPlayer = packet.toPlayer,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
                ExtraBytes = packet.extraBytes,
            });
        }

        internal void ToPlayersPacket(ToPlayersPacket packet)
        {
            RecieveData(new EventContainer
            {
                IsReliable = packet.IsReliable,
                SceneName = packet.sceneName,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
                ExtraBytes = packet.extraBytes,
            });
        }

        internal bool HasServerCounterPart(string modName)
        {
            if (ServerPipeList.Contains(modName))
            {
                return true;
            }
            return false;
        }

        internal void HasServerCounterPart(string modName, Action<bool> callback)
        {
            if (HasServerPipeList)
            {
                callback(ServerPipeList.Contains(modName));
            }
            else
            {
                Send<GetServerMetadataPacket>(PacketsEnum.GetServerMetadataPacket, new GetServerMetadataPacket
                {
                    MetadataKey = (byte)METADATA.INSTALLED_PIPES
                });

                void x(object _, EventArgs e)
                {
                    if (HasServerPipeList)
                    {
                        callback(ServerPipeList.Contains(modName));
                        OnMetadataUpdate -= x;
                    }
                }

                OnMetadataUpdate += x;
            }
        }

    }
}