using System;
using System.Numerics;

namespace NVMP.Entities
{
    /// <summary>
    /// Characters are a specialized Actor, that only players will ever use. These objects cannot be created via the SDK, and every player
    /// that joins the server will always be allocated a character. They allow you to control additional PlayerCharacter specific behaviours, 
    /// whilst also acting with the same interface as an Actor and Reference.
    /// </summary>
    public interface INetCharacter : INetActor
    {
        /// <summary>
        /// The current spectator mode the player is playing with. 
        /// </summary>
        public NetCharacterSpectateMode SpectatingMode { get; set; }

        /// <summary>
        /// Configures what encounters the player will sync to the server. See IEncounterConfig for additional documentation.
        /// </summary>
        public INetCharacterEncounterConfig EnabledEncounters { get; }

        /// <summary>
        /// Current position of the spectate mode.
        /// </summary>
        public Vector3 SpectatePosition { get; set; }

        /// <summary>
        /// Current rotation of the spectate mode.
        /// </summary>
        public Quaternion SpectateRotation { get; set; }

        /// <summary>
        /// Specifies the entity the player will spectate/orbit around if the spectating mode is on a focus entity.
        /// </summary>
        public INetReference SpectateEntity { get; set; }

        /// <summary>
        /// The reference the character is currently aiming at. This only applies to synchronised references aimed at, 
        /// unsynced entities will not appear on this synced data.
        /// </summary>
        public NetAbstractReference CrosshairReference { get; }

        /// <summary>
        /// How far away from an entity the spectate mode will be. This is disregarded if the player is not spectating an entity.
        /// </summary>
        public float SpectateDistance { get; set; }

        /// <summary>
        /// How fast the spectate mode moves for the player when using movement keys.
        /// </summary>
        public float SpectateSpeed { get; set; }

        /// <summary>
        /// Modifies if the player can use player driven fast travel. As of NVMPx4, this flag does not work.
        /// </summary>
        public bool CanFastTravel { get; set; }

        /// <summary>
        /// Modifies if the player can use TCL (essentially no clip).
        /// </summary>
        public bool HasTCL { get; set; }
    }
}
