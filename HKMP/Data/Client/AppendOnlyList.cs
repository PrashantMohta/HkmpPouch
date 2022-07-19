using HkmpPouch;
using Newtonsoft.Json;
namespace HkmpPouch.PouchDataClient{

    public class AppendOnlyList{
        public string Name;
        public List<string> listData = new();
        public event EventHandler<AppendOnlyListUpdateEventArgs> OnUpdate;
        private HkmpPipe pipe;

        public AppendOnlyList(HkmpPipe pipe,string name){
            this.pipe = pipe;
            this.Name = name;
            pipe.OnRecieve += HandleEvent;
        }

        public void HandleEvent(System.Object _, RecievedEventArgs args){
            var packet = args.packet;
            if(!packet.eventData.StartsWith($"{Name}|")){ return; }
            if(packet.eventName == AppendOnlyListEvents.ADDED){
                var data = packet.eventData.Split(new Char[] {'|'},2);
                var item = data[1];
                listData.Add(item);
                OnUpdate?.Invoke(this,new AppendOnlyListUpdateEventArgs{
                    data = this.listData
                });
                
            }
            if(packet.eventName == AppendOnlyListEvents.GOTALL){
                var data = packet.eventData.Split(new Char[] {'|'},2);
                listData = JsonConvert.DeserializeObject<List<string>>(data[1]);
                OnUpdate?.Invoke(this,new AppendOnlyListUpdateEventArgs{
                    data = this.listData
                });
            }
        }

        public void Add(string item,int ttl){
            // send add command to server
            this.pipe.Send(0,0,AppendOnlyListEvents.ADD,$"{this.Name}|{ttl}|{item}",false,false,true,false);
        }

        public void GetAll(){
            // send getAll command to server
            this.pipe.Send(0,0,AppendOnlyListEvents.GETALL,this.Name,false,false,true,false);
        }
        public void Destroy(){
            pipe.OnRecieve -= HandleEvent;
        }

    }
}