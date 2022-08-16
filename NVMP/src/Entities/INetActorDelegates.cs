using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP.Entities
{
    public delegate void OnDeath
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetActorMarshaler))]
                INetActor attacker
    );
}
