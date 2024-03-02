using Hkmp.Api.Server;
using Hkmp.Api.Server.Networking;
using Hkmp.Networking.Packet;
using HkmpPouch.DataStorage;
using HkmpPouch.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

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

        internal Dictionary<string, List<IServerPlayer>> PipeToPlayersMap = new();
        internal Dictionary<ushort,List<string>> PlayerIdToPipeMap = new();
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
            Api.ServerManager.PlayerDisconnectEvent += ServerManager_PlayerDisconnectEvent;
            NetReceiver = Api.NetServer.GetNetworkReceiver<PacketsEnum>(Instance,PacketBoi.InstantiatePacket);

            NetReceiver.RegisterPacketHandler<GetServerMetadataPacket>(PacketsEnum.GetServerMetadataPacket, GetServerMetadataPacketHandler);
            NetReceiver.RegisterPacketHandler<RegisterPipePacket>(PacketsEnum.RegisterPipePacket, RegisterPipePacketHandler);

            NetReceiver.RegisterPacketHandler<ToServerPacket>(PacketsEnum.ToServerPacket, ToServerPacketHandler);
            NetReceiver.RegisterPacketHandler<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, PlayerToPlayerPacketHandler);
            NetReceiver.RegisterPacketHandler<PlayerToPlayersPacket>(PacketsEnum.PlayerToPlayersPacket, PlayerToPlayersPacketHandler);

        }

        private void ServerManager_PlayerDisconnectEvent(IServerPlayer player)
        {
            RemovePlayerPipeRegisterations(player);
        }

        private void RemovePlayerPipeRegisterations(IServerPlayer player)
        {
            foreach(var pipe in PipeToPlayersMap)
            {
                if (pipe.Value.Contains(player))
                {
                    pipe.Value.Remove(player);
                }
            }
            if (PlayerIdToPipeMap.ContainsKey(player.Id))
            {
                PlayerIdToPipeMap.Remove(player.Id);
            }   
        }

        private void RegisterPipePacketHandler(ushort fromPlayer, RegisterPipePacket packet)
        {
            var player = Api.ServerManager.GetPlayer(fromPlayer);
            if (!PipeToPlayersMap.TryGetValue(packet.modName, out _))
            {
                PipeToPlayersMap[packet.modName] = new();
            }
            if (!PipeToPlayersMap[packet.modName].Contains(player))
            {
                PipeToPlayersMap[packet.modName].Add(player);
            }

            if (!PlayerIdToPipeMap.TryGetValue(fromPlayer, out _))
            {
                PlayerIdToPipeMap[fromPlayer] = new();
            }
            if (!PlayerIdToPipeMap[fromPlayer].Contains(packet.modName))
            {
                PlayerIdToPipeMap[fromPlayer].Add(packet.modName);
            }
        }

        private void GetServerMetadataPacketHandler(ushort playerId, GetServerMetadataPacket packet)
        {

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
                ExtraBytes = packet.extraBytes
            });
        }

        internal void PlayerToPlayerPacketHandler(ushort fromPlayer, PlayerToPlayerPacket packet)
        {
            // rebroadcast
            packet.fromPlayer = fromPlayer;

            if (PlayerIdToPipeMap.TryGetValue(fromPlayer, out var pipes))
            {
                if (pipes.Contains(packet.mod))
                {
                    Send<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, packet, packet.toPlayer);
                }
            } else
            {
                Send<PlayerToPlayerPacket>(PacketsEnum.PlayerToPlayerPacket, packet, packet.toPlayer);
            }
        }

        internal void PlayerToPlayersPacketHandler(ushort fromPlayer, PlayerToPlayersPacket packet)
        {
            
            // rebroadcast
            bool allScenes = packet.sceneName == Constants.AllScenes;
            bool sameScene = packet.sceneName == Constants.SameScenes;

            if(!PipeToPlayersMap.TryGetValue(packet.mod, out var players))
            {
                players = (List<IServerPlayer>)Api.ServerManager.Players;
            }
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
                    eventName = packet.eventName,
                    extraBytes = packet.extraBytes,
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

            if (!PipeToPlayersMap.TryGetValue(packet.mod, out var players))
            {
                players = (List<IServerPlayer>)Api.ServerManager.Players;
            }
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