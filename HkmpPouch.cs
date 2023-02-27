using Hkmp.Api.Client;
using Hkmp.Api.Server;
using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace HkmpPouch
{
    public class HkmpPouch : Mod
    {
        new public string GetName() => Constants.Name;
        public override string GetVersion() => Constants.ActualVersion;

        internal static HkmpPouch Instance;

        internal static ClientAddon client;
        internal static ServerAddon server;
        private static void RegisterAddons()
        {
            if (client == null)
            {
                client = new Client();
                ClientAddon.RegisterAddon(client);
            }
            if (server == null)
            {
                server = new Server();
                ServerAddon.RegisterAddon(server);
            }
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            if (Instance == null)
            {
                Instance = this;
                RegisterAddons();
            }
        }
    }
}