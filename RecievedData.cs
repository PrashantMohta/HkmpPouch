using System;

namespace HkmpPouch
{

    /// <summary>
    /// EventArgs that are used for PipeClient.OnRecieve and PipeServer.OnRecieve
    /// </summary>
    public class ReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The received data
        /// </summary>
        public EventContainer Data { get; set; }
    }
    
}
