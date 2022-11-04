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
    public enum ImGuiTreeNodeFlags
    {
        None                 = 0,
        Selected             = 1 << 0,   // Draw as selected
        Framed               = 1 << 1,   // Draw frame with background (e.g. for CollapsingHeader)
        AllowItemOverlap     = 1 << 2,   // Hit testing to allow subsequent widgets to overlap this one
        NoTreePushOnOpen     = 1 << 3,   // Don't do a TreePush() when open (e.g. for CollapsingHeader) = no extra indent nor pushing on ID stack
        NoAutoOpenOnLog      = 1 << 4,   // Don't automatically and temporarily open node when Logging is active (by default logging will automatically open tree nodes)
        DefaultOpen          = 1 << 5,   // Default node to be open
        OpenOnDoubleClick    = 1 << 6,   // Need double-click to open node
        OpenOnArrow          = 1 << 7,   // Only open when clicking on the arrow part. If ImGuiTreeNodeFlags_OpenOnDoubleClick is also set, single-click arrow or double-click all box to open.
        Leaf                 = 1 << 8,   // No collapsing, no arrow (use as a convenience for leaf nodes).
        Bullet               = 1 << 9,   // Display a bullet instead of arrow
        FramePadding         = 1 << 10,  // Use FramePadding (even for an unframed text node) to vertically align text baseline to regular widget height. Equivalent to calling AlignTextToFramePadding().
        SpanAvailWidth       = 1 << 11,  // Extend hit box to the right-most edge, even if not framed. This is not the default in order to allow adding other items on the same line. In the future we may refactor the hit system to be front-to-back, allowing natural overlaps and then this can become the default.
        SpanFullWidth        = 1 << 12,  // Extend hit box to the left-most and right-most edges (bypass the indented area).
        NavLeftJumpsBackHere = 1 << 13,  // (WIP) Nav: left direction may move to this TreeNode() from any of its child (items submitted between TreeNode and TreePop)
        CollapsingHeader     = Framed | NoTreePushOnOpen | NoAutoOpenOnLog
    };

}
