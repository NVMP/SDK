using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities
{
	/// <summary>
	/// Actors are world NPCs that can interact with other players, and are built from pre-defined form IDs within the ESMs loaded by the players.
	/// </summary>
    public interface INetActor : INetReference
	{
		/// <summary>
		/// In Fallout: New Vegas, this is "Caps". This is just a helper class for referencing the internal money that is used by NPCs to
		/// barter and value items at. 
		/// </summary>
		public static readonly NetActorInventoryItem GameCurrency = NetActorInventoryItem.GetByReference(0xF);

		/// <summary>
		/// The sex flag of this actor.
		/// </summary>
		public bool IsFemale { get; set; }

		/// <summary>
		/// The knock down state of the actor, ie. ragdolled or knocked down.
		/// </summary>
		public NetActorKnockedState KnockedState { get; set; }

		/// <summary>
		/// Queries the current playing idle the actor is performing
		/// </summary>
		public uint PlayingIdle { get; }

		/// <summary>
		/// Querys if this actor a player character. Not to be confused with TESCharacter. INetActor's that have this set can be
		/// safely casted to INetCharacter
		/// </summary>
		public bool IsCharacter { get; }

		/// <summary>
		/// The gravity multiplier this actor is set with
		/// </summary>
		public float GravityMult { get; set; }

		/// <summary>
		/// Whether the actor will be able to take damage or not
		/// </summary>
		public bool HasGodmode { get; set; }

		/// <summary>
		/// A readonly value of the actor's lifestate
		/// </summary>
		public bool IsDead { get; }

		/// <summary>
		/// The actor's game level
		/// </summary>
		public int Level { get; set; }

		/// <summary>
		/// The actor's overriden level.
		/// </summary>
		public string LevelOverride { get; set; }

        /// <summary>
        /// Current health on the actor
        /// </summary>
        public float Health { get; set; }

		/// <summary>
		/// Maximum health the character can reach
		/// </summary>
		public float MaxHealth { get; set; }

        /// <summary>
        /// The active armor on the actor. This must be set within the maximum armor.
        /// </summary>
        public float Armor { get; set; }

        /// <summary>
        /// Maximum armor seen on the actor. Setting this then requires you to update the armor value.
        /// </summary>
        public float MaxArmor { get; set; }

		/// <summary>
		/// XP currently awarded to this actor. This is only supported on characters!
		/// </summary>
		public int XP { get; set; }

		/// <summary>
		/// An interface helper that will automatically adjust the Caps inventory item to the specified value, or query it's contents.
		/// </summary>
		public uint Caps { get; set; }

		/// <summary>
		/// Restrains or unrestrians the player, this prevents movement of players - and prevents AI from running on NPCs.
		/// </summary>
		/// <remarks>
		/// This can't be used on dead actors.
		/// </remarks>
		public bool IsRestrained { get; set; }

		/// <summary>
		/// Queries or sets if the actor is aiming their weapon (ironsights).
		/// </summary>
		public bool IsAiming { get; set; }

		/// <summary>
		/// Controls or queries the movement flags on the NPC. Combine flags to move the actor in the direction desired relevent
		/// to their aim direction.
		/// </summary>
		public NetActorMovementFlags MovementFlags { get; set; }

		/// <summary>
		/// Controls or queries the movement speed flags on the NPC.
		/// </summary>
		public NetActorSpeedFlags MovementSpeedFlags { get; set; }

		/// <summary>
		/// Kills the actor.
		/// </summary>
		public void Kill();

		/// <summary>
		/// Resurrects the actor.
		/// </summary>
		public void Resurrect();

        /// <summary>
        /// A collection of projectiles that will ignore all server armor when applied to this actor via damage.
        /// </summary>
        public uint[] ArmorIgnoredProjectiles { get; set; }

        /// <summary>
        /// A collection of weapons that will ignore all server armor when applied to this actor via damage.
        /// </summary>
        public uint[] ArmorIgnoredWeapons { get; set; }

        /// <summary>
        /// Returns the number of inventory items on this actor
        /// </summary>
        /// <returns></returns>
        public uint GetNumItems();

		/// <summary>
		/// Returns all inventory items on this actor
		/// </summary>
		/// <returns></returns>
		public NetActorInventoryReference[] GetItems();

		/// <summary>
		/// Finds a form in the container with the specified form ID
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		public NetActorInventoryReference? GetItemByForm(uint form);

        /// <summary>
		/// Finds items that have the specified biped flag.
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public NetActorInventoryReference[] GetItemsByBiped(BipedFlag flag);

        /// <summary>
        /// Finds a form in the container with the specified form ID
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public NetActorInventoryReference? GetItemByForm(NetActorInventoryItem item);

		/// <summary>
		/// Adds a new inventory item to this actor's inventory
		/// </summary>
		/// <param name="item">item reference to add</param>
		/// <param name="count">num to stack</param>
		/// <param name="equipped">equipped or not</param>
		public void AddItem(NetActorInventoryItem item, uint count = 1, bool equipped = false);

		/// <summary>
		/// Adds a collection of items to this actor's inventory
		/// </summary>
		/// <param name="items"></param>
		/// <param name="equipped"></param>
		public void AddItems(NetActorInventoryItem[] items, bool equipped = false);

		/// <summary>
		/// Removes an item from this actor's inventory with the specified item form
		/// </summary>
		/// <param name="item"></param>
		/// <param name="count"></param>
		public void RemoveItem(NetActorInventoryItem item, uint count = 1);

		/// <summary>
		/// Removes all items from this actor's inventory
		/// </summary>
		public void RemoveAllItems();

		/// <summary>
		/// Summons an animation on the actor using the specified idle form.
		/// </summary>
		/// <param name="formID"></param>
		public void PlayIdle(uint formID);

		/// <summary>
		/// Stops all animations on the actor.
		/// </summary>
		public void StopIdle();

		/// <summary>
		/// Summons an AI package on the actor.
		/// </summary>
		/// <param name="package"></param>
		public void AddPackage(INetActorPackage package);

		/// <summary>
		/// Clears all AI targets on the actor.
		/// </summary>
		public void ClearTargets();

		/// <summary>
		/// Adds a target to the AI of this actor.
		/// </summary>
		/// <param name="target"></param>
		public void AddTarget(INetActor target);

		/// <summary>
		/// Sets the specified actor value on the actor, setting the live value. If this is a skill based actor value, 
		/// then the actor may adjust their base value with the same call.
		/// </summary>
		/// <param name="av"></param>
		/// <param name="value"></param>
		public void SetActorValue(NetActorValues av, float value);

		/// <summary>
		/// Sets the specified actor base value on the actor.
		/// </summary>
		/// <param name="av"></param>
		/// <param name="value"></param>
		public void SetBaseActorValue(NetActorValues av, float value);

		/// <summary>
		/// Returns the set actor value on the actor.
		/// </summary>
		/// <param name="av"></param>
		/// <returns></returns>
		public float GetActorValue(NetActorValues av);

		/// <summary>
		/// Returns the set base actor value on the actor.
		/// </summary>
		/// <param name="av"></param>
		/// <returns></returns>
		public float GetBaseActorValue(NetActorValues av);

		/// <summary>
		/// Resets all actor values. Not guarenteed to be the base form values, but the network specified values (usually ZERO).
		/// </summary>
		public void ResetActorValues();

		/// <summary>
		/// Fires a weapon from the actor's perspective.
		/// </summary>
		/// <param name="weaponFormID">Must be provided in order to link the correct projectile and damage stats</param>
		/// <param name="originPoint"></param>
		/// <param name="rotation">this gets adjusted to the actors angle in most cases</param>
		/// <param name="projectileFormIDOverride">If set, the specified projectile is uses instead of the weapons default projectile.</param>
		public void FireWeapon(uint weaponFormID, Vector3 originPoint, Quaternion rotation, uint projectileFormIDOverride = uint.MinValue);

		/// <summary>
		/// A delegate you can bind to track when this actor dies.
		/// </summary>
		public event OnDeath Died;

        /// <summary>
        /// A delegate you can bind to track when this 
        /// </summary>
        public event OnAttack Attack;
    }
}
