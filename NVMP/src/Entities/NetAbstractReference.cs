using System;
using System.Collections.Generic;
using System.Text;

namespace NVMP.Entities
{
    /// <summary>
    /// In some parts of NV:MP, the server can receive events the player is interacting with that may be unsynced. An example may be when a player 
    /// activated a TESObjectDOOR that is not synced, or was killed by a reference that is not synced on the network, but exists and has a unique ref ID tag.
    /// 
    /// This up to you to do what you desire at implementation level. Most of the time it makes sense only to handle references that are synchronised, as it means
    /// you can edit the state of the other reference, or query more information. Whereas on the other hand, you may just want to track what reference ID was involved
    /// in a specific interaction that doesn't need more context.
    /// 
    /// For RADD, I implemented this because I want to track what doors player's interact with - without having to sync every door that ever existed around the player.
    /// </summary>
    public struct NetAbstractReference
    {
        /// <summary>
        /// The synchronised reference involved in the interaction. This may be null, and should be null checked before accessing as abstract
        /// references may only contain the reference ID.
        /// </summary>
        public INetReference NetReference { get; internal set; }

        /// <summary>
        /// The reference ID based in this interaction. This will always be set.
        /// </summary>
        public uint RefID { get; internal set; }

        /// <summary>
        /// The reference base ID based in this interaction. This will always be set.
        /// </summary>
        public uint BaseID { get; internal set; }

        /// <summary>
        /// The reference form type based in this interaction.
        /// </summary>
        public NetReferenceFormType RefFormType { get; internal set; }

        /// <summary>
        /// The extralist active type bitset of the reference based in this interaction.
        /// </summary>
        public ReadOnlyExtraDataList RefExtraDataList { get; internal set; }
    }
}
