using System;
using System.Numerics;

namespace NVMP.Entities
{
    /// <summary>
    /// WorldSpace objects contain an entire Fallout worldspace state. They are either transient, or non transient. Transient worldspaces
    /// are passed around to players to synchronise, and other players must receive data about that worldspace. Non-transient run various
    /// primary sync states on the server, such as current time - and weather overrides. Transient worldspaces will destroy if no one is
    /// in the worldspace any longer, and non-transient persist forever.
    /// </summary>
    public interface INetWorldSpace
    {
        /// <summary>
        /// The worldspace form ID.
        /// </summary>
        public WorldspaceType FormID { get; }

        /// <summary>
        /// Returns whether this worldspace is in a transient state. Transient worldspaces are passed around to player to synchronise, and
        /// non-transient are always server owned - and override the world state.
        /// </summary>
        public bool IsTransient { get; set; }

        /// <summary>
        /// Returns the current weather in use by the world space. To change the weather, use the transition or override helper.
        /// </summary>
        public uint WeatherCurrent { get; }

        /// <summary>
        /// Returns the previous weather in use by the world space, used as part of transitions.
        /// </summary>
        public uint WeatherPrevious { get; }

        /// <summary>
        /// Returns or sets the weather override for the world space. 
        /// </summary>
        public uint WeatherOverride { get; set; }

        /// <summary>
        /// Returns or sets the current time on the server (represented by the hour of the day). The date information is not 
        /// used other than calculating the current hour of the day in use.
        /// Non-transient world spaces will automatically update their times for clients to continue simulation on, and transients
        /// will update their time remotely - without interpolation on this value!
        /// </summary>
        public DateTime Time { get; set; }

        //
        // Methods
        //

        /// <summary>
        /// Transitions the worldspace to use the target weather ID. A transition may either be instant, or over time depending on
        /// client's current summary of their world space.
        /// </summary>
        /// <param name="targetWeatherID"></param>
        public void TransitionToWeather(uint targetWeatherID);
    }
}
