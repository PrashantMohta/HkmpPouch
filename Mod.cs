using Hkmp.Api.Client;
using Hkmp.Api.Server;

namespace HkmpPouch
{
    public class HkmpPouch : Mod{

        new public string GetName() => Constants.Name;
        public override string GetVersion() => Constants.Version;

        public static event EventHandler<EventArgs> OnReady;

        public static HkmpPouch Instance;
        public static ClientAddon client;
        public static ServerAddon server;
        private static void RegisterAddons(){
            if(client == null){
                client = new Client();
                ClientAddon.RegisterAddon(client);
            }
            if(server == null){
                server = new Server();
                ServerAddon.RegisterAddon(server);
            }
            OnReady?.Invoke(HkmpPouch.Instance,new EventArgs());
        }
        public override void Initialize()
        {
            if (Instance == null) 
            { 
                Instance = this;
                RegisterAddons();
            }
        }
    }
}