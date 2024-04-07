using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HkmpPouch
{
    /// <summary>
    /// Basic data All events must have
    /// </summary>
    public class baseEventData
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

        /// <summary>
        /// Extra Bytes sent with the event
        /// </summary>
        public byte[] ExtraBytes = new byte[0];
    }

    /// <summary>
    /// The Type Denoting an event
    /// </summary>
    public abstract class PipeEvent : baseEventData
    {
        /// <summary>
        /// Gets the Name of the Event
        /// </summary>
        /// <returns>The name</returns>
        public abstract string GetName();


        /// <summary>
        /// Convert the event into it's string representation
        /// </summary>
        /// <returns>string</returns>
        public abstract override string ToString();


    }
    /// <summary>
    /// A Factory that can generate PipeEvents given the correct string representation
    /// </summary>
    public interface IEventFactory
    {
        /// <summary>
        /// Gets the Name of the Event
        /// </summary>
        /// <returns>The name</returns>
        public string GetName();

        /// <summary>
        /// Used to instantiate a PipeEvent from a string representation
        /// </summary>
        /// <param name="serializedData">string representation</param>
        /// <returns>the PipeEvent</returns>
        public PipeEvent FromSerializedString(string serializedData);

    }

    /// <summary>
    /// Data received
    /// </summary>
    public class EventContainer : baseEventData
    {

        /// <summary>
        /// Deserialised Event object from event data using an IEventFactory
        /// </summary>
        public PipeEvent Event;
    }

}
