using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    /// <summary>
    /// A window template is a sealed/immutable window template layout that allows you to present custom user interface 
    /// to a player, with custom logic for different elements.
    /// </summary>
    public interface IGUIWindowTemplate
    {
        /// <summary>
        /// The identifier for the window. 
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// The window title presented to the player.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The specific ImGui window flags bound.
        /// </summary>
        public ImGuiWindowFlags ImGuiFlags { get; }

        /// <summary>
        /// Controls whether the window can be closed/dismissed by the player via the menu bar at the top.
        /// </summary>
        public bool CanBeClosed { get; }

        /// <summary>
        /// Controls whether the window requires input focus when created, as opposed to only optionally interacted when the game chat is focused.
        /// </summary>
        public bool RequiresInputFocus { get; }

        /// <summary>
        /// Optional forced position where the window will be rendered
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// Optional dimensions to render the window in
        /// </summary>
        public Vector2 Dimensions { get; }

        /// <summary>
        /// Elements part of this window template.
        /// </summary>
        public IList<IGUIBaseElement> Elements { get; }

        /// <summary>
        /// Finds an element either at root element, or recursively in subchildren
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IGUIBaseElement FindElementByID(ulong id);
    }

}
