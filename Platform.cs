using System.IO;
namespace HkmpPouch{
    public static class Platform{
        
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
        public static void Log(string message){
            try{
                LogMapi(message);
            } catch (Exception e){
                LogServer($"[INFO] {message}");
            }
        }
        public static void LogDebug(string message){
            if(!currentSettings.enableDebugLogging){ return; }
            try{
                LogDebugMapi(message);
            } catch (Exception e){
                LogServer($"[DEBUG] {message}");
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