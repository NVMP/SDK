using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP.Entities.Encoding
{
    public class ReferenceDataVistor
    {
        #region Natives

        [DllImport("Native", EntryPoint = "GameNetReference_AllocateEncodedData")]
        internal static extern IntPtr Internal_AllocateEncodedData();

        [DllImport("Native", EntryPoint = "GameNetReference_ReleaseEncodeData")]
        internal static extern void Internal_ReleaseEncodeData(IntPtr encodedDataEntry);

        [DllImport("Native", EntryPoint = "GameNetReference_GetEncodedDataEntryName")]
        internal static extern string Internal_GetEncodedDataEntryName(IntPtr encodedDataEntry);

        [DllImport("Native", EntryPoint = "GameNetReference_GetEncodedDataEntryData")]
        internal static extern string Internal_GetEncodedDataEntryData(IntPtr encodedDataEntry);

        [DllImport("Native", EntryPoint = "GameNetReference_SetEncodedDataEntryName")]
        internal static extern void Internal_SetEncodedDataEntryName(IntPtr encodedDataEntry, string data);

        [DllImport("Native", EntryPoint = "GameNetReference_SetEncodedDataEntryData")]
        internal static extern void Internal_SetEncodedDataEntryData(IntPtr encodedDataEntry, string data);

        [DllImport("Native", EntryPoint = "GameNetReference_GetIsHead")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool Internal_GetIsHead(IntPtr encodedDataEntry);

        [DllImport("Native", EntryPoint = "GameNetReference_SetIsHead")]
        internal static extern void Internal_SetIsHead(IntPtr encodedDataEntry, bool value);

        [DllImport("Native", EntryPoint = "GameNetReference_GetChildEntry")]
        internal static extern IntPtr Internal_GetChildEntry(IntPtr encodedDataEntry);

        [DllImport("Native", EntryPoint = "GameNetReference_SetChildEntry")]
        internal static extern void Internal_SetChildEntry(IntPtr encodedDataEntry, IntPtr childPtr);

        [DllImport("Native", EntryPoint = "GameNetReference_GetNext")]
        internal static extern IntPtr Internal_GetNext(IntPtr encodedDataEntry);

        [DllImport("Native", EntryPoint = "GameNetReference_SetNext")]
        internal static extern void Internal_SetNext(IntPtr encodedDataEntry, IntPtr childPtr);

        #endregion

        [JsonConstructor]
        public ReferenceDataVistor()
        {
            __UnmanagedAllocatedEncoding = Internal_AllocateEncodedData();
        }

        // The address of the unmanaged data this interface marshals against
        [JsonIgnore]
        public IntPtr __UnmanagedAllocatedEncoding { get; set; }

        public virtual bool IsVisitor()
        {
            return true;
        }

        public string Name
        {
            get
            {
                return __UnmanagedAllocatedEncoding != IntPtr.Zero ? Internal_GetEncodedDataEntryName(__UnmanagedAllocatedEncoding) : null;
            }
            set
            {
                Internal_SetEncodedDataEntryName(__UnmanagedAllocatedEncoding, value);
            }
        }

        public string Data
        {
            get
            {
                return __UnmanagedAllocatedEncoding != IntPtr.Zero ? Internal_GetEncodedDataEntryData(__UnmanagedAllocatedEncoding) : null;
            }
            set
            {
                Internal_SetEncodedDataEntryData(__UnmanagedAllocatedEncoding, value);
            }
        }


        public ReferenceDataVistor Child
        {
            get
            {
                IntPtr refPtr = Internal_GetChildEntry(__UnmanagedAllocatedEncoding);
                if (refPtr != IntPtr.Zero)
                {
                    return new ReferenceDataVistor
                    {
                        __UnmanagedAllocatedEncoding = refPtr
                    };
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    Internal_SetChildEntry(__UnmanagedAllocatedEncoding, value.__UnmanagedAllocatedEncoding);
                }
                else
                {
                    IntPtr child = Internal_GetChildEntry(__UnmanagedAllocatedEncoding);
                    if (child != IntPtr.Zero)
                    {
                        Internal_SetChildEntry(__UnmanagedAllocatedEncoding, IntPtr.Zero);
                        Internal_ReleaseEncodeData(child);
                    }
                }
            }
        }

        public ReferenceDataVistor Next
        {
            get
            {
                IntPtr refPtr = Internal_GetNext(__UnmanagedAllocatedEncoding);
                if (refPtr != IntPtr.Zero)
                {
                    return new ReferenceDataVistor
                    {
                        __UnmanagedAllocatedEncoding = refPtr
                    };
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    Internal_SetNext(__UnmanagedAllocatedEncoding, value.__UnmanagedAllocatedEncoding);
                }
                else
                {
                    IntPtr next = Internal_GetNext(__UnmanagedAllocatedEncoding);
                    if (next != IntPtr.Zero)
                    {
                        Internal_SetNext(__UnmanagedAllocatedEncoding, IntPtr.Zero);
                        Internal_ReleaseEncodeData(next);
                    }
                }
            }
        }

        [JsonIgnore]
        public bool IsHead
        {
            get
            {
                return Internal_GetIsHead(__UnmanagedAllocatedEncoding);
            }
            set
            {
                Internal_SetIsHead(__UnmanagedAllocatedEncoding, value);
            }
        }
    }

    public class ReferenceData : ReferenceDataVistor, IDisposable
    {
        public static ReferenceData Create()
        {
            return (ReferenceData)Marshals.EncodedReferenceDataMarshaler.GetInstance(null)
                .MarshalNativeToManaged(Internal_AllocateEncodedData());
        }

        public class JsonEncodedEntry
        {
            public string Name { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

            public string Data { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

            public List<JsonEncodedEntry> Items { get; set; }
        }

        static internal List<JsonEncodedEntry> ConvertEncodedEntryToJsonEntry(ReferenceDataVistor data)
        {
            var items = new List<JsonEncodedEntry>();

            ReferenceDataVistor node = data;
            while (node != null)
            {
                var item = new JsonEncodedEntry
                {
                    Name = node.Name,
                    Data = node.Data?.Length > 0 ? node.Data : null,
                };

                ReferenceDataVistor child = node.Child;
                if (child != null)
                {
                    item.Items = ConvertEncodedEntryToJsonEntry(child);
                }

                items.Add(item);

                node = node.Next;
            }

            return items;
        }

        protected List<JsonEncodedEntry> CachedItems;
        protected bool IsDisposed = false;

        public override bool IsVisitor()
        {
            return false;
        }

        [JsonIgnore]
        public List<JsonEncodedEntry> Items
        {
            get
            {
                if (CachedItems == null)
                {
                    CachedItems = ConvertEncodedEntryToJsonEntry(this);
                }
                return CachedItems;
            }
            set
            {
                CachedItems = value;
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                // Visitors are not responsible for the unmanaged side of this
                if (__UnmanagedAllocatedEncoding != IntPtr.Zero)
                {
                    if (!IsVisitor())
                    {
                        Internal_ReleaseEncodeData(__UnmanagedAllocatedEncoding);
                    }
                }

                __UnmanagedAllocatedEncoding = IntPtr.Zero;
                IsDisposed = true;
            }
        }

        ~ReferenceData()
        {
            Dispose();
        }
    }
}
