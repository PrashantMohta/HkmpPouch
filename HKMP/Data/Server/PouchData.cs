namespace HkmpPouch.PouchDataServer{
     public class PouchData{
        public string modName;
        private Dictionary<string,Counter> Counters = new();
        private Dictionary<string,AppendOnlyList> AppendOnlyLists = new();

        public PouchData(string modName){
            this.modName = modName;
            Server.Instance.OnRecieve += HandleEvent;
        }
        public void HandleEvent(System.Object _, RecievedEventArgs args){
            
            if(Counters == null){
                Counters = new();
            }
            if(AppendOnlyLists == null){
                AppendOnlyLists =  new();
            }

            var packet = args.packet;
            if(packet.mod != modName){ return; }
            Platform.LogDebug($"{packet.mod} PouchData recieve {packet.eventName} = {packet.eventData}");

            if(packet.eventName == CounterEvents.INCREMENT || packet.eventName == CounterEvents.DECREMENT || packet.eventName == CounterEvents.GET){
                Counter c;
                var counterName = packet.eventData;
                if(!Counters.TryGetValue(counterName,out c)){
                    c = new Counter(packet.mod,counterName);
                    Counters[counterName] = c;
                }
                if(packet.eventName == CounterEvents.GET){
                    c.UpdateClient(packet.fromPlayer);
                }
                if(packet.eventName == CounterEvents.INCREMENT){
                    c.Increment();
                }
                if(packet.eventName == CounterEvents.DECREMENT){
                    c.Decrement();
                }
            }
            if(packet.eventName == AppendOnlyListEvents.ADD || packet.eventName == AppendOnlyListEvents.GETALL){
                AppendOnlyList a;
                var data = packet.eventData.Split(new Char[] {'|'},3);
                var listName = data[0];
                if(!AppendOnlyLists.TryGetValue(listName,out a)){
                    a = new AppendOnlyList(packet.mod,listName);
                    AppendOnlyLists[listName] = a;
                }
                if(packet.eventName == AppendOnlyListEvents.ADD){
                    a.Add(new ListItem{
                        insertedOn = DateTime.Now, 
                        ttl = Int32.Parse(data[1]),
                        value = data[2]
                    });
                }
                if(packet.eventName == AppendOnlyListEvents.GETALL){
                    Modding.Logger.Log("WE HAVE THE EVENT ON SERVER");
                    a.UpdateClientWithAllData(packet.fromPlayer);
                }
            }
        }
    }
}