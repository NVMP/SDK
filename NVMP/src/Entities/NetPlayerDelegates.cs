using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP.Entities
{
    internal static class NetPlayerDelegates
    {
        internal delegate void OnPresentedInteraction
        (
            [In] uint uWindowId,
            [In] ulong uElementId,
            [In] NetPlayer.MenuUpdateType menuUpdateType
        );
    }
}
