using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    /// <summary>
    /// This is taken from ImGui, as the GUI templating uses ImGui as it's primary driver. 
    /// </summary>
    public enum ImGuiWindowFlags
    {
        None                   = 0,
        NoTitleBar             = 1 << 0,
        NoResize               = 1 << 1,   // Disable user resizing with the lower-right grip
        NoMove                 = 1 << 2,   // Disable user moving the window
        NoScrollbar            = 1 << 3,   // Disable scrollbars (window can still scroll with mouse 
        NoScrollWithMouse      = 1 << 4,   // Disable user vertically scrolling with mouse wheel. On 
        NoCollapse             = 1 << 5,   // Disable user collapsing window by double-clicking on it
        AlwaysAutoResize       = 1 << 6,   // Resize every window to its content every frame
        NoBackground           = 1 << 7,   // Disable drawing background color (WindowBg, etc.) and o
        NoSavedSettings        = 1 << 8,   // Never load/save settings in .ini file
        NoMouseInputs          = 1 << 9,   // Disable catching mouse, hovering test with pass through
        MenuBar                = 1 << 10,  // Has a menu-bar
        HorizontalScrollbar    = 1 << 11,  // Allow horizontal scrollbar to appear (off by default). 
        NoFocusOnAppearing     = 1 << 12,  // Disable taking focus when transitioning from hidden to 
        NoBringToFrontOnFocus  = 1 << 13,  // Disable bringing window to front when taking focus (e.g
        AlwaysVerticalScrollbar= 1 << 14,  // Always show vertical scrollbar (even if ContentSize.y <
        AlwaysHorizontalScrollbar=1<< 15,  // Always show horizontal scrollbar (even if ContentSize.x
        AlwaysUseWindowPadding = 1 << 16,  // Ensure child windows without border uses style.WindowPa
        NoNavInputs            = 1 << 18,  // No gamepad/keyboard navigation within the window
        NoNavFocus             = 1 << 19,  // No focusing toward this window with gamepad/keyboard na
        UnsavedDocument        = 1 << 20,  // Display a dot next to the title. When used in a tab/doc
        NoNav                  = NoNavInputs | NoNavFocus,
        NoDecoration           = NoTitleBar | NoResize | NoScrollbar | NoCollapse,
        NoInputs               = NoMouseInputs | NoNavInputs | NoNavFocus,
                
        NavFlattened           = 1 << 23,  // [BETA] Allow gamepad/keyboard navigation to cross over 
        ChildWindow            = 1 << 24,  // Don't use! For internal use by BeginChild()
        Tooltip                = 1 << 25,  // Don't use! For internal use by BeginTooltip()
        Popup                  = 1 << 26,  // Don't use! For internal use by BeginPopup()
        Modal                  = 1 << 27,  // Don't use! For internal use by BeginPopupModal()
        ChildMenu              = 1 << 28   // Don't use! For internal use by BeginMenu()
    }
}
