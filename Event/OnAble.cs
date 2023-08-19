using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HkmpPouch
{
    /// <summary>
    /// Used to chainably listen to events
    /// </summary>
    public class OnEvent
    {

        private event EventHandler<EventContainer> handler;

        private List<Action<PipeEvent>> thenActions = new List<Action<PipeEvent>>();

        internal IEventFactory eventFactory;

        private bool destroyed = false;

        /// <summary>
        /// Bool determining if the OnEvent is already destroyed
        /// </summary>
        public bool Destroyed { get => destroyed; private set => destroyed = value; }

        /// <summary>
        /// Constructs the OnEvent
        /// </summary>
        public OnEvent() {
            handler += OnEvent_handler;
        }
        /// <summary>
        /// Trigger the event
        /// </summary>
        /// <param name="Data">Data to trigger the event with</param>
        public void TriggerWith(EventContainer Data)
        {
            if (!Destroyed) { 
                handler?.Invoke(this, Data);
            }
        }
        

        private void OnEvent_handler(object sender, EventContainer e)
        {
            if(eventFactory != null)
            {
                e.Event = eventFactory.FromSerializedString(e.EventData);
                e.Event.IsReliable = e.IsReliable;
                e.Event.ModName = e.ModName;
                e.Event.EventName = e.EventName;
                e.Event.EventData = e.EventData;
                e.Event.FromPlayer = e.FromPlayer;
                e.Event.ToPlayer = e.ToPlayer;
                e.Event.SceneName = e.SceneName;
                e.Event.ExtraBytes = e.ExtraBytes;
            }
            thenActions.ForEach(x => x(e.Event));
        }




        /// <summary>
        /// Do the callback once the first matching event is triggered
        /// </summary>
        /// <param name="Callback">Callback to run</param>
        /// <returns>this OnEvent for chaining</returns>
        public OnEvent DoOnce(Action<PipeEvent> Callback)
        {
            handler += OnEvent_do_Once_handler;
            void OnEvent_do_Once_handler(object sender, EventContainer e)
            {
                if (eventFactory != null)
                {
                    e.Event = eventFactory.FromSerializedString(e.EventData);
                    e.Event.IsReliable = e.IsReliable;
                    e.Event.ModName = e.ModName;
                    e.Event.EventName = e.EventName;
                    e.Event.EventData = e.EventData;
                    e.Event.FromPlayer = e.FromPlayer;
                    e.Event.ToPlayer = e.ToPlayer;
                    e.Event.SceneName = e.SceneName;
                    e.Event.ExtraBytes = e.ExtraBytes;
                }
                Callback(e.Event);
                handler -= OnEvent_do_Once_handler;
            }
            return this;
        }

        /// <summary>
        /// Do the callback every time the a matching event is triggered
        /// </summary>
        /// <param name="Callback">Callback to run</param>
        /// <returns>this OnEvent for chaining</returns>
        public OnEvent Do<T>(Action<T> Callback) where T : PipeEvent
        {
            thenActions.Add((PipeEvent e) => Callback((T)e));
            return this;
        }

        /// <summary>
        /// Do the callback every time the a matching event is triggered
        /// </summary>
        /// <param name="Callback">Callback to run</param>
        /// <returns>this OnEvent for chaining</returns>
        public OnEvent Do(Action<PipeEvent> Callback)
        {
            thenActions.Add(Callback);
            return this;
        }

        /// <summary>
        /// Removes all existing listeners from the OnEvent
        /// </summary>
        public void Destroy()
        { 
            handler = null;
            Destroyed = true;
        }

    }
    
    /// <summary>
    /// OnAble class that allows for Events handling using callbacks
    /// </summary>
    public class OnAble
    {
        private Dictionary<string, List<OnEvent>> _events = new();

        /// <summary>
        /// Listen to Events of type EventContainer with a given Event Factory
        /// </summary>
        /// <param name="Factory">name of the event to Listen to</param>
        /// <returns>returns an OnEvent object that allows you to respond to the event</returns>
        public OnEvent On(IEventFactory Factory)
        {
            var EventName = Factory.GetName();
            if (!_events.TryGetValue(EventName, out var v))
            {
                _events[EventName] = new();
            }
            var onEvent = new OnEvent();
            onEvent.eventFactory = Factory;
            _events[EventName].Add(onEvent);

            return onEvent;
        }


        /// <summary>
        /// Used to actually trigger the events that your Listeners can recieve
        /// </summary>
        /// <param name="EventName">Name of the event</param>
        /// <param name="RecievedEvent">Event payload</param>
        protected void TriggerEvents(string EventName, EventContainer RecievedEvent)
        {
            if (_events.TryGetValue(EventName, out var onEvent))
            {
                onEvent.ForEach((onE) => onE.TriggerWith(RecievedEvent));
            }
        }

        /// <summary>
        /// Used to dispose off all the existing listeners
        /// </summary>
        protected void Destroy()
        {
            foreach(var kvp in _events)
            {
                kvp.Value.ForEach((onE) => onE.Destroy());
            }
            _events = new();
        }

    }
}
