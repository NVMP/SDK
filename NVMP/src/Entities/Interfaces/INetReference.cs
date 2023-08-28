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
        /// Defines the 3D model scale of the reference. Fallout internally limits this between 0.001-10.000.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// The 3D Quaternion rotation of the reference in the world. If the object is attached, this rotation will
        /// not apply to the target reference, and you should use LocalRotation instead.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// The 3D Quaternion local rotation of the reference in the world. Applying this is relative to the object, 
        /// and will affect the outcome of the transformed rotation by the local axis of the reference.
        /// </summary>
        public Quaternion LocalRotation { get; set; }

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
        /// Sets the renderable invisibility flag. Setting this flag will still sync the network contents and create a rendered NiNode, but the
        /// object will not be visible until this flag is removed.
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

        /// <summary>
        /// A utility function for warping a reference to the specified marker ID.
        /// Note: This function may not work if the object is not currently replicated on a player's machine, and will throw
        /// an exception if so.
        /// </summary>
        /// <param name="markerID"></param>
        public void TeleportToMarker(string markerID);

        /// <summary>
        /// A utility function for warping a reference directly to another reference in the game.
        /// </summary>
        /// <param name="other"></param>
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

        /// <summary>
        /// Serializes the object to an encodable structure that can be decoded back into the object.
        /// </summary>
        /// <returns></returns>
        public ReferenceData Encode();

        /// <summary>
        /// Deserializes previously encoded data for this object type and restores all object state.
        /// </summary>
        /// <param name="entry"></param>
        public void Decode(ReferenceData entry);

        /// <summary>
        /// Serializes the object to an encodable structure, and then converts it to JSON format.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Serializing to JSON is not safe, nor performant. If the native data structure changes, this serialization will break big time. ")]
        public string EncodeToJSON();

        /// <summary>
        /// Deserializes the object from JSON, and restores its state.
        /// </summary>
        /// <param name="json"></param>
        [Obsolete("Serializing to JSON is not safe, nor performant. If the native data structure changes, this serialization will break big time. ")]
        public void DecodeFromJSON(string json);

        /// <summary>
        /// Serializes the entire objects state to a byte buffer internally stored, that can be rstored with ReloadState. This is much faster
        /// than calling Encode/Decode due to the fact the object state is written to an underlying sync buffer, which never needs marshalling.
        /// </summary>
        public void SaveState();

        /// <summary>
        /// Deserializes a previously saved state, restoring the object back entirely.
        /// </summary>
        public void ReloadState();

        /// <summary>
        /// A utility function for querying whether a reference is of an actor type.
        /// </summary>
        [Obsolete("Use `(is INetActor)` instead as object relationships are safe to test against. ")]
        public bool IsActor { get; }

        /// <summary>
        /// Returns whether the object is in a dormant time state. This happens if the owner of the object replicating it (defined by PlayerOwner) has
        /// tabbed out of the game, entered a game menu that freezes the game, or has lost connection.
        /// </summary>
        public bool IsIdle { get; }

        /// <summary>
        /// A utility helper that tests if another reference is in the potentially-visible-set of this object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsInPVS(INetReference other);

        /// <summary>
        /// A utility helper that tests if the current reference is inside of an INetZone. This function is designed to be significantly
        /// performant, as AABB bounds tests are pre-calculated on objects moving within PVS sets preemptively.
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public bool IsInZone(INetZone zone);

        /// <summary>
        /// Returns a list of zones this reference is inside of. It's better to use IsInZone for directly checking, as it avoids a lot of
        /// scanning and searching as it will just use internal pool index matching. Use NumZonesInside for just checking sizes, it avoids the
        /// same overhead. 
        /// </summary>
        public INetZone[] ZonesInside { get; }

        /// <summary>
        /// Returns the number of zones this reference is inside of. 
        /// </summary>
        public uint NumZonesInside { get; }

        /// <summary>
        /// The implemented PVS controller.
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
        /// Sets or gets flags specific to attachment behaviour.
        /// </summary>
        public AttachmentFlags ParentAttachmentFlags { get; set; }

        /// <summary>
        /// Sets the parent attachment offset. This is a position with its origin relative to the parent attachment's live position, and
        /// is transformed as the parent reference changes rotation additionally - so this is a local space position.
        /// </summary>
        public Vector3 ParentAttachmentOffset { get; set; }

        /// <summary>
        /// Sets the parent attachment rotation. This is relative to the object in use.
        /// </summary>
        public Quaternion ParentAttachmentRotation { get; set; }

        /// <summary>
        /// Sets the node name the attachment will attach to. If this is null, or is invalid, the parent NiNode will be used by default.
        /// </summary>
        public string ParentAttachmentNodeName { get; set; }

        /// <summary>
        /// Returns the current sound file being played. To play a sound on a reference, use the methods as this property is readonly.
        /// </summary>
        public string CurrentSound { get; }


        /// <summary>
        /// Plays a sound on the reference. If any other sounds have been played, they are immediately stopped and replaced
        /// with the new request. You can also call StopSound directly to remove all playback.
        /// </summary>
        /// <param name="request">request</param>
        public void PlaySound(NetSoundRequest request);

        /// <summary>
        /// Removes any sounds being played on the reference.
        /// </summary>
        public void StopSound();

        /// <summary>
        /// Destroys the reference. Using flags controls additional behaviour.
        /// </summary>
        /// <param name="flags"></param>
        public void Destroy(NetReferenceDeletionFlags flags = 0);
    }
}
