using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP.Entities
{
    public delegate bool OnActivatedReference
        (
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetReferenceMarshaler))]
                    INetReference reference

            , [In] uint refId
        );
}
