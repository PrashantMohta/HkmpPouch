using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ReceivedData Data { get; set; }
    }
    /// <summary>
    /// Data received
    /// </summary>
    public class ReceivedData
    {
        //metadata
        /// <summary>
        /// If the packet was marked IsReliable
        /// </summary>
        public bool IsReliable = false;

        //actual data
        /// <summary>
        /// Name of the scene the data was meant to be sent to
        /// </summary>
        public string SceneName = "";

        /// <summary>
        /// The Id of the player the data was sent to
        /// </summary>
        public ushort ToPlayer = 0;

        /// <summary>
        /// The Id of the player the data was sent from
        /// </summary>
        public ushort FromPlayer = 0;

        /// <summary>
        /// Name of the Pipe
        /// </summary>
        public string ModName = "";

        /// <summary>
        /// Name of the Event
        /// </summary>
        public string EventName = "";

        /// <summary>
        /// Corresponding EventData
        /// </summary>
        public string EventData = "";
    }
}
