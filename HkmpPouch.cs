using Hkmp.Api.Client;
using Hkmp.Api.Server;
using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace HkmpPouch
{
    /// <summary>
    /// The Mod class for loading in game
    /// </summary>
    public class HkmpPouch : Mod
    {

        internal static HkmpPouch Instance;
        internal static ClientAddon client;
        internal static ServerAddon server;

        /// <summary>
        /// Name of the Mod
        /// </summary>
        /// <returns>Name of the Mod</returns>
        new public string GetName() => Constants.Name;
        /// <summary>
        /// Version of the mod
        /// </summary>
        /// <returns>Version of the mod</returns>
        public override string GetVersion() => Constants.ActualVersion;

        /// <summary>
        /// The Initialize function for this mod
        /// </summary>
        /// <param name="preloadedObjects">preloadedObjects</param>
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            if (Instance == null)
            {
                Instance = this;
                RegisterAddons();
            }
        }

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

    }
}