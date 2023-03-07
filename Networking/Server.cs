using Hkmp.Api.Server;
using Hkmp.Api.Server.Networking;
using Hkmp.Networking.Packet;
using HkmpPouch.DataStorage;
using HkmpPouch.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HkmpPouch.Networking
{
    internal class Server : ServerAddon, ILogger
    {
        public override bool NeedsNetwork => true;

        protected override string Name => Constants.Name;

        protected override string Version => Constants.Version;

        internal static Server Instance;

        internal IServerApi Api;

        internal event EventHandler<ReceivedEventArgs> OnRecieve;
        internal IServerAddonNetworkReceiver<PacketsEnum> NetReceiver;
        internal IServerAddonNetworkSender<PacketsEnum> NetSender;

        internal Dictionary<string, DataStorageServerManager> ModDataStorageServerManager = new Dictionary<string, DataStorageServerManager>();
        private List<string> PipeList = new();

        internal void AddPipe(string s)
        {
            PipeList.Add(s);
        }
        public Server()
        {
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


        public override void Initialize(IServerApi serverApi)
        {
            Api = serverApi;
            NetReceiver = Api.NetServer.GetNetworkReceiver<PacketsEnum>(Instance,PacketBoi.InstantiatePacket);

            NetReceiver.RegisterPacketHandler<GetServerMetadataPacket>(PacketsEnum.GetServerMetadataPacket, GetServerMetadataPacketHandler);

            NetReceiver.RegisterPacketHandler<ToServerPacket>(PacketsEnum.ToServerPacket, ToServerPacketHandler);
            NetReceiver.RegisterPacketHandler<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, PlayerToPlayerPacketHandler);
            NetReceiver.RegisterPacketHandler<PlayerToPlayersPacket>(PacketsEnum.PlayerToPlayersPacket, PlayerToPlayersPacketHandler);

        }

        private void GetServerMetadataPacketHandler(ushort playerId, GetServerMetadataPacket packet)
        {

            Logger.Info("got get packet " + PipeList.Count);
            Send<ServerPipeListPacket>(PacketsEnum.ServerPipeListPacket, new ServerPipeListPacket
            {
                PipeList = PipeList
            }, playerId);
        }

        internal void ToServerPacketHandler(ushort fromPlayer, ToServerPacket packet)
        {
            if (!ModDataStorageServerManager.TryGetValue(packet.mod, out var DataStore)) {
                ModDataStorageServerManager[packet.mod] = new DataStorageServerManager(packet.mod);
            }
            RecieveData(new EventContainer
            {
                IsReliable = packet.IsReliable,
                FromPlayer = fromPlayer,
                ModName = packet.mod,
                EventData = packet.eventData,
                EventName = packet.eventName,
            });
        }

        internal void PlayerToPlayerPacketHandler(ushort fromPlayer, PlayerToPlayerPacket packet)
        {
            // rebroadcast
            packet.fromPlayer = fromPlayer;
            Send<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, packet, packet.toPlayer);
        }

        internal void PlayerToPlayersPacketHandler(ushort fromPlayer, PlayerToPlayersPacket packet)
        {
            
            // rebroadcast
            bool allScenes = packet.sceneName == Constants.AllScenes;
            bool sameScene = packet.sceneName == Constants.SameScenes;

            var players = Api.ServerManager.Players;
            var sender = Api.ServerManager.GetPlayer(fromPlayer);
            packet.fromPlayer = fromPlayer;

            for (var i = 0; i < players.Count; i++)
            {
                var player = players.ElementAt(i);
                if(player.Id != sender.Id && (allScenes || (player.CurrentScene == sender.CurrentScene && sameScene) || player.CurrentScene == packet.sceneName))
                {
                    Send<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, new PlayerToPlayerPacket { 
                    _isReliable = packet._isReliable,
                    toPlayer = player.Id,
                    fromPlayer = fromPlayer,
                    sceneName = packet.sceneName,
                    mod = packet.mod,
                    eventData = packet.eventData,
                    eventName = packet.eventName
                    
                    }, player.Id);
                }
            }
               

        }

        internal void Send<TPacketData>(PacketsEnum PacketId, TPacketData PacketData, ushort ToPlayer) where TPacketData : IPacketData, new()
        {
            if (!Api.NetServer.IsStarted)
            {
                return;
            }
            if (NetSender == null)
            {
                NetSender = Api.NetServer.GetNetworkSender<PacketsEnum>(Instance);
            }
            NetSender.SendCollectionData(PacketId, PacketData, ToPlayer);
        }

        internal void Broadcast(ToPlayersPacket packet) {
            bool allScenes = packet.sceneName == Constants.AllScenes;

            var players = Api.ServerManager.Players;

            for (var i = 0; i < players.Count; i++)
            {
                var player = players.ElementAt(i);
                if ( (allScenes|| player.CurrentScene == packet.sceneName))
                {
                    Send<ToPlayersPacket>(PacketsEnum.ToPlayersPacket, packet, player.Id);
                }
            }
        }
    }
}