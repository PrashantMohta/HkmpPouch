using System.IO;
namespace HkmpPouch{
    public static class Platform{

        private static bool _isServer = true,_serverChecked = false;

        private static bool CheckIfClient(){
            return typeof(UnityEngine.GameObject) != null;
        }

        /// <summary>
        /// check if the mod is running on server or client
        /// </summary>
        /// <returns></returns>
        public static bool isServer(){
            if(!_serverChecked){ 
                try{
                    CheckIfClient();
                    _isServer = false;
                } catch {
                    _isServer = true;
                }
                _serverChecked = true;
            }
            return _isServer;
        }
        
        private static void LogServer(string message){
            if(currentSettings.enableLogging){
                Console.WriteLine(message);
            }
        }
        private static void LogMapi(string message){
            HkmpPouch.Instance.Log(message);
        }
        private static void LogDebugMapi(string message){
            HkmpPouch.Instance.LogDebug(message);
        }

        /// <summary>
        /// Log general messages
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message){
            if(isServer()){
                LogServer($"[INFO] {message}");
            } else {
                LogMapi(message);
            }
        }
        /// <summary>
        /// Log messages for debugging
        /// </summary>
        /// <param name="message"></param>
        public static void LogDebug(string message){
            if(!currentSettings.enableDebugLogging){ return; }
            if(isServer()){
                LogServer($"[DEBUG] {message}");
            } else {
                LogDebugMapi(message);
            }
        }


        /// <summary>
        /// Get the directory path of the Calling Assembly
        /// </summary>
        /// <returns></returns>
        public static string getCurrentDirectory(){
           return Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
        }
    }
}