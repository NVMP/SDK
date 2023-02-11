using Newtonsoft.Json;
using NVMP.Entities.Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
    /// <summary>
    /// References are networked TESObjectREFR's. They support basic synchronisation functionality, along with specific networking
    /// configurations where needed. This interface is inherited by other classes that depend on additional functionality.
    /// </summary>
    public interface INetReference : IEquatable<INetReference>, IDisposable
    {
        /// <summary>
        /// A helper to query Fallout: New Vegas's upwards vector.
        /// </summary>
        public static readonly Vector3 UpVector = new Vector3(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// The default virtual world ID all objects are synchronised in when created.
        /// </summary>
        public static readonly uint DefaultVirtualWorldID = 0;

        /// <summary>
        /// Base form ID of this reference. If refID is not set, this is used to instanciate a new reference of this form ID.
        /// </summary>
        public uint FormID { get; }

        /// <summary>
        /// The mod file name this reference FormID is part of.
        /// </summary>
        public string ModFile { get; }

        /// <summary>
        /// The GECK form type this reference is. This is only reliable if it has had an existing owner
        /// beforehand. Server references that have never been synced by another player 
        /// may not have this set at all as the server cannot infer it alone.
        /// </summary>
        public NetReferenceFormType FormType { get; }

        /// <summary>
        /// The unique reference ID of this reference. This can only be set on creation to ensure all clients in PVS use the correct
        /// reference ID.
        /// </summary>
        public uint RefID { get; }

        /// <summary>
        /// The display name of the actor. This is additionally set as the actor's form name, 
        /// so may have direct Fallout manipulations. Maximum name of 255 characters is permitted. 
        /// Clients may not change the name of actors.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sets whether the reference be interacted with the E key. For actors this may be either dialogue, or looting.
        /// For containers, this is looting. And for other stuff, just check the GECK.
        /// </summary>
        public bool Activatable { get; set; }

        /// <summary>
        /// Sets the actor's simulation type. See NetReferenceSimulationType for options. Default value for SDK references is SimulationType.Static
        /// </summary>
        public NetReferenceSimulationType SimType { get; set; }

        /// <summary>
        /// A delegate you can bind to track when this reference activates another reference. If you return false on the delegate, it will
        /// reject the activation on the activator's machine
        /// </summary>
        public event OnActivatedReference ActivatedOtherReference;

        /// <summary>
        /// A delegate you can bind to track when this refrence is damaged by another actor. 
        /// </summary>
        /// <remarks>
        /// This currently only works for INetActors, and will throw an exception for now if you try and use it for non-actors.
        /// </remarks>
        public event OnDamaged Damaged;

        /// <summary>
        /// A delegate you can bind to track when this reference is activated by another reference. If you return false on the delegate, it will
        /// reject the activation on the activator's machine.
        /// </summary>
        public event OnActivatedReference Activated;

        /// <summary>
        /// If an name has been set, this changes the colour of it in RGBA (0-255) format
        /// </summary>
        public Color NameColor { get; set; }

        /// <summary>
        /// The overhead title of the actor. 
        /// Clients may not change the title of actors.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// If an overhead title has been set, this changes the colour of it in RGBA (0-255) format
        /// </summary>
        public Color TitleColor { get; set; }

        /// <summary>
        /// Objects that are inside the same virtual world are synchronised together, but if they are not then they have no understanding 
        /// of eachother's state other than existance. By default all objects are DEFAULT_VWID
        /// </summary>
        public uint VirtualWorldID { get; set; }

        /// <summary>
        /// The 3D coordinates of the actor in the world
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The 3D coordinates of the actor in the world
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// The 3D Quaternion rotation of the actor in the world
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// The player who currently owns this actor for synchronisation
        /// </summary>
        public INetPlayer PlayerOwner { get; set; }

        /// <summary>
        /// Currently only positions the yaw axis
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(INetReference target);

        /// <summary>
        /// Currently only positions the yaw axis
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(Vector3 target);

        /// <summary>
        /// Sets the network PVS invisibility flag. This makes the entity not visible and becomes unsynced to all players. Changes made
        /// to this object won't be applied to player's machines until it becomes visible again.
        /// </summary>
        public bool IsInvisible { get; set; }

        /// <summary>
        /// An exterior cell information object. Should check for !IsInInterior before accesisng this
        /// to be sure this data is in use. 
        /// </summary>
        public WorldspaceCoordinate Worldspace { get; }

        /// <summary>
        /// The interior cell ID, or zero if in an exterior.
        /// </summary>
        public uint Interior { get; }

        /// <summary>
        /// Returns if the reference is inside an interior, if not then they are in a worldspace cell.
        /// </summary>
        public bool IsInInterior { get; }

        /// <summary>
        /// Teleports the actor to a specific cell ID. Can be used in pair with Position, Rotation
        /// Do note that the formID must be an interior cell, worldspace cells will come up short if the player has not loaded that region - in which
        /// case you should be using TeleportToExterior
        /// </summary>
        /// <param name="formid"></param>
        public void SetCell(uint formid);

        public void TeleportToMarker(string markerID);

        public void TeleportTo(INetReference other);

        /// <summary>
        /// Teleports the actor to an exterior cell. Can be used in pair with Position, Rotation
        /// </summary>
        /// <param name="worldspace">worldspace area to spawn into</param>
        /// <param name="x">grid x coord</param>
        /// <param name="y">grid y coord</param>
        public void SetExterior(WorldspaceType worldspace, int x, int y);

        /// <summary>
        /// Teleports the actor to an exterior cell. If position is set, the CELL X and Y component on teleport 
        /// is automatically computed to the desired position to move to. The referenec is also moved into the
        /// specified position.
        /// 
        /// This is important to use if you are moving a player, as for LOD reasons the X and Y component must be
        /// set up appropriately on warp.
        /// </summary>
        /// <param name="worldspace"></param>
        /// <param name="position"></param>
        public void SetExterior(WorldspaceType worldspace, Vector3 position);

        public ReferenceData Encode();

        public void Decode(ReferenceData entry);

        public string EncodeToJSON();


        public void SaveState();

        public void ReloadState();

        public void DecodeFromJSON(string json);

        public bool IsActor { get; }

        public bool IsIdle { get; }

        public bool IsInPVS(INetReference other);

        public bool IsInZone(INetZone zone);

        /// <summary>
        /// The implemented PVS controller
        /// </summary>
        public INetReferencePVSController PVS { get; }

        /// <summary>
        /// Returns whether an object is destroyed in unmanaged code. This doesn't mean that the object is free'd from memory,
        /// but is actually waiting for a deletion when all available handles are free.
        /// 
        /// If you are handling NetReference handles, you should periodically check this to then release your object to the
        /// GC collector - else the console will warn about your access being prolongued after death.
        /// </summary>
        public bool IsDestroyed { get; }

        /// <summary>
        /// Sets or gets the reference this reference is attached on to. References cannot attach to eachother, the link is one direction to allow
        /// for multiple attachments to one reference. Note that this instantly will update the position of the reference relative to the parent, 
        /// with the local offset adjusted for.
        /// </summary>
        public INetReference ParentAttachment { get; set; }

        /// <summary>
        /// Sets the parent attachment offset. This is a position with its origin relative to the parent attachment's live position, and
        /// is transformed as the parent reference changes rotation additionally - so this is a local space position.
        /// </summary>
        public Vector3 ParentAttachmentOffset { get; set; }

        /// <summary>
        /// Destroys the reference. Using flags controls additional behaviour.
        /// </summary>
        /// <param name="flags"></param>
        public void Destroy(NetReferenceDeletionFlags flags = 0);
    }
}
