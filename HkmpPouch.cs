using Hkmp.Api.Client;
using Hkmp.Api.Server;
using HkmpPouch.Networking;
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
        /// LoadPriority of the Mod
        /// </summary>
        /// <returns></returns>
        public override int LoadPriority() => Constants.Priority;

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
        /// ctor
        /// </summary>
        public HkmpPouch()
        {
            if (client == null)
            {
                client = new Client();
            }
            if (server == null)
            {
                server = new Server();
            }
        }

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
            ClientAddon.RegisterAddon(client);
            ServerAddon.RegisterAddon(server);
        }

    }
}