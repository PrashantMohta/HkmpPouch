using HkmpPouch;
namespace HkmpPouch.PouchDataClient{

    public class Counter{
        public string Name;
        public int Count {get; private set;}
        public event EventHandler<CounterUpdateEventArgs> OnUpdate;
        private HkmpPipe pipe;

        public Counter(HkmpPipe pipe,string name){
            this.pipe = pipe;
            this.Name = name;
            this.Count = 0;
            pipe.OnRecieve += HandleEvent;
        }

        public void HandleEvent(System.Object _, RecievedEventArgs args){
            var packet = args.packet;
            if(packet.eventName == CounterEvents.UPDATE){
                var data = packet.eventData.Split('|');
                if(Name == data[0]) {
                    this.Count = Int32.Parse(data[1]);
                    OnUpdate?.Invoke(this,new CounterUpdateEventArgs{
                        Count = this.Count
                    });
                }
            }
        }

        public void Get(){
            this.pipe.Send(0,0,CounterEvents.GET,this.Name,false,false,true,false);
        }
        public void Increment(){
            // send increment command to server
            this.pipe.Send(0,0,CounterEvents.INCREMENT,this.Name,false,false,true,false);
        }

        public void Decrement(){
            // send decrement command to server
            this.pipe.Send(0,0,CounterEvents.DECREMENT,this.Name,false,false,true,false);
        }

        public void Destroy(){
            pipe.OnRecieve -= HandleEvent;
        }
    }
    
}