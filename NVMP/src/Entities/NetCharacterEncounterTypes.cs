namespace NVMP.Entities
{
    public enum NetCharacterEncounterTypes : uint
    {
        /// <summary>
        /// The player will synchronise actors they encounter if this is set to true.
        /// </summary>
        EncounterActors,

        /// <summary>
        /// The player will synchronise references they encounter if this is set to true.
        /// </summary>
        EncounterReference
    }
}
