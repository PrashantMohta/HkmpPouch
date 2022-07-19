using Hkmp.Api.Server;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;
using System.Linq;
using HkmpPouch.PouchDataServer;

namespace HkmpPouch{
    public class Server : ServerAddon {
        public static Server Instance;
        public IServerApi serverApi;
        internal event EventHandler<RecievedEventArgs> OnRecieve;
        
        public Server() {
            LoadSettings();
            Instance = this;
        }
        internal void SendToAll(string _mod,string _eventName,string _eventData,bool _reliable = false){
            var players = serverApi.ServerManager.Players;
            for(var i = 0; i < players.Count ; i++){
                var player = players.ElementAt(i);
                this.Send(0,player.Id,_mod,_eventName,_eventData,false,true,_reliable);
            }
        }
        internal void SendToAll(ushort fromPlayer,string _mod,string _eventName,string _eventData,bool _reliable = false,bool sameScene = false){
            var players = serverApi.ServerManager.Players;
            var sender = serverApi.ServerManager.GetPlayer(fromPlayer);
            for(var i = 0; i < players.Count ; i++){
                var player = players.ElementAt(i);
                if(player.Id != fromPlayer && (!sameScene || sender.CurrentScene == player.CurrentScene)){
                    this.Send(fromPlayer,player.Id,_mod,_eventName,_eventData,false,true,_reliable);
                }
            }
        }

        internal void Send(ushort fromPlayer,ushort toPlayer,string _mod,string _eventName,string _eventData,bool _rebroadcast = false , bool _broadcastToAll = false ,bool _reliable = false){
            /*if(!serverApi.NetServer.IsStarted ){
                return;
            }*/
            Platform.LogDebug($" {_mod} Server send {_eventName} : {_eventData} to player {toPlayer}");
            var netSender = serverApi.NetServer.GetNetworkSender<Packets>(this);
            // SendCollectionData using the given packet ID
            netSender.SendCollectionData(Packets.GenericPacket, new GenericPacket {
               _isReliable = _reliable,
               _dropReliableDataIfNewerExists = false,
               mod = _mod,
               rebroadcast = _rebroadcast,
               broadcastToAll = _broadcastToAll,
               fromPlayer = fromPlayer,
               toPlayer = toPlayer,
               eventName = _eventName,
               eventData = _eventData
            },toPlayer);
        }
        internal Dictionary<string,PouchData> ModPouchData = new ();
        internal void PouchDataHandler(GenericPacket packet){
            PouchData pd;
            var modName = packet.mod;
            if(!ModPouchData.TryGetValue(modName,out pd)){
                pd = new PouchData(modName);
                ModPouchData[modName] = pd;
            }
        }
        public override void Initialize(IServerApi serverApi) {
            this.serverApi = serverApi;
            var netReceiver = serverApi.NetServer.GetNetworkReceiver<Packets>(this,InstantiatePacket);

            netReceiver.RegisterPacketHandler<GenericPacket>(
                Packets.GenericPacket,
                (id, packetData) => {
                    packetData.fromPlayer = id; // we just set this here because HKMP does not tell clients their own id for whatever reason.
                    Platform.LogDebug($"{packetData.mod} Server recieve {packetData.eventName} : {packetData.eventData} from player {id}");

                    //handle pouch data
                    PouchDataHandler(packetData);

                    //broadcast the packet to all server addons
                    OnRecieve?.Invoke(this,new RecievedEventArgs{
                        packet = packetData
                    });

                    //rebroadcast the packet to all clients 
                    if(packetData.rebroadcast){
                        if(packetData.broadcastToAll){
                            SendToAll(id,packetData.mod,packetData.eventName,packetData.eventData,packetData._isReliable,packetData.sameScene);
                        } else {
                            Send(id,packetData.toPlayer,packetData.mod,packetData.eventName,packetData.eventData,false,packetData._isReliable);
                        }
                    }
                }
            );
            
            Platform.Log($"Pouch Server Addon Version {Constants.ActualVersion} Ready");
        }


        protected override string Name => Constants.Name;
        protected override string Version => Constants.Version;
        public override bool NeedsNetwork => true;
        private static IPacketData InstantiatePacket(Packets packetId) {
            switch (packetId) {
                case Packets.GenericPacket:
                    return new PacketDataCollection<GenericPacket>();
            }
            return null;
        }
    }
}