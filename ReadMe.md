# HkmpPouch

Pouch is an Addon meant to serve as an intermediary layer between HKMP API and Networked Addons that want to remain optional on either the client side and/or the server side without any loss of functionality.

Pouch does not re-implement the HKMP API (nor do I have any intentions to do so) you will still probably need a dependency on HKMP itself for most Add-ons, especially on the server side where HKMP server is what loads your server side addon.

**Core Features** : 

 - **Peer to Peer communication**:
	 Any Server with Pouch installed onto it can be used as a "relay"    to allow clients to communicate with each other without the need for    writing a server side addon, this allows for simple message passing    between clients where you can target a single player, all players in    a scene or all players connected to the server.

 - **Optional Addons with networking**:
   HKMP supports optional addons as long as they do not require networking, this is because HKMP's protocol depends on the client addon to decode the data sent by the server (and vice-versa).  Pouch allows you to circumvent this limitation by using Pouch's networking access, and communicating in a few standard packets that can deliver arbitrary strings to denote data. By serializing and deserializing your events as strings you can do Server - Client addon communication for even the most complex mods while remaining optional on both the sides.

 - **Server side temporary data storage**
   Clients can store some data on the server across clients and disconnections, currently 2 types of data are supported.

	 - **Integer counters** :
        addons can increment or decrement it, the idea is to not depend on the counter's previous value when doing this, to avoid race conditions, this is why it does not support a direct "change value", instead it can be used to track the number of times certain events happen (kills, pogos, hits -- whatever you need)
        
      - **Timed AppendOnlyLists** :
        This is a list of string items on to which you can append more strings, with a TTL of some seconds. This can then be used to persist something in the state of the "world" for a certain duration ( or till the server restarts ). for example this is how I persist skulls of players after they are killed. 


Note : Due to the way this works, it means that Pouch in itself becomes a mandatory mod for joining the servers that have it, there's no going around this unfortunately.


**Existing example mods** : 

 - Server - Client communication 
	 - [ESoulLink](https://github.com/PrashantMohta/ESoulLink)
 - P2P communication
	 - [GhostHunter](https://github.com/PrashantMohta/ghosthunter)
	 - [EmoteWheel](https://github.com/PrashantMohta/EmoteWheel)
	 - [HKMP.HealthDisplay](https://github.com/TheMulhima/HKMP.HealthDisplay) 
 - Server side data storage
	 - [MultiplayerEvents](https://github.com/PrashantMohta/MultiplayerEvents)

