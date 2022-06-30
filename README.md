

# NV:MP Software Development Kit Source

**Attention: If you are looking to download the SDK kit, please visit the releases section of this repository!**
It is not recommended to download the source code if you have no intention of recompiling the NVMP.dll or it's sister libraries. 

This is the source code for NV:MP's SDK libraries, primarily the NVMP.dll. It encapsulates all native objects, provides interfaces to communicating to the native program, and implements common  services such as server reporting and mod download web helpers.

This page is mainly for reference, and to allow community driven improvements to the SDK.

## Documentation
The documentation website that is inside this source code can be accessed here:
https://nv-mp.com/sdk

## Unamanged-managed Relationships
Most native objects encapsulated in the SDK are provided via interfaces to managed objects associated to the data. Every native object provided to the C# engine constructs its own managed object twin. This is an important relationship to be aware of, as depending on how objects were created - the way they may want to cleanup may differ.

Objects created in the SDK (Factory.Actor.Create, etc) will exist until any of these conditions are met:
1. They get disposed of in the garbage collector. Losing all references to an object created via managed code will tell the native application to clean it up.
2. The object is simulated by players as a temporary simulation type. If you expect the reference to self-destruct if the owner simulating it requests it, then you should periodically check the IsDestroyed member and remove your references. If you fail to do this, the server will spew warnings into it's console window and the object won't be destroyed until it all C# handles are free'd.
3. A managed piece of code calls Destroy() on the managed object, queuing it for deletion. 


Objects created by the native server (passive reference encounters, player INetCharacter's, etc) will exist until any of these conditions are met:
1. A managed piece of code calls Destroy() on the managed object, queuing it for deletion.
2. The native server destroys it for any unspecified reason (player disconnect, cleaned up on the simulated players list, etc)
 
This does mean that objects created via the managed factories from managed code have more freedom, as you can pass the object around as a disposable object. However if you tag them for various simulation configurations, they could suddenly tag themselves for destruction - check the documentation for the simulation types in use.

