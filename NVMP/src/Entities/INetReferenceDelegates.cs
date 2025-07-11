﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NVMP.Entities
{
    internal class ExtraDataListBitsetMarshalerToBitArray : ICustomMarshaler
    {
        public static ICustomMarshaler Instance { get; } = new ExtraDataListBitsetMarshalerToBitArray();

        public static ICustomMarshaler GetInstance(string pstrCookie)
            => new ExtraDataListBitsetMarshalerToBitArray();

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            throw new NotImplementedException();
        }

        public int GetNativeDataSize()
        {
            return 0;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            return IntPtr.Zero;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                return null;
            }

            var data = new byte[21];
            Marshal.Copy(pNativeData, data, 0, 21);

            return new ReadOnlyExtraDataList(data);
        }
    }

    public class ReadOnlyExtraDataList
    {
        internal BitArray _bytes;

        public ReadOnlyExtraDataList(byte[] existingSet)
        {
            _bytes = new BitArray(existingSet);
        }

        /// <summary>
        /// Returns if the specified extra data type exists in this extra data list
        /// </summary>
        /// <param name="type">The type bit to check</param>
        /// <returns></returns>
        public bool IsSet(NetReferenceExtraDataType type)
        {
            return _bytes.Get((int)type);
        }
    }

    public delegate NetReferencePVSTestTypes OnPVSCheck
        (
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                    INetPlayer player
        );

    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool OnActivatedReference
        (
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetReferenceMarshaler))]
                    INetReference activator

            , [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetReferenceMarshaler))]
                    INetReference target

            , [In] uint refId
            , [In] uint baseId
            , [In] NetReferenceFormType formType
            , [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ExtraDataListBitsetMarshalerToBitArray))] ReadOnlyExtraDataList extraDataList
        );

    public delegate void OnDamaged
        (
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetReferenceMarshaler))]
                    INetReference victim
        
            , [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetActorMarshaler))]
                    INetActor attacker

            , [In] float healthDamage
            , [In] float armorDamage
            , [In] uint weaponFormId
            , [In] uint projectileFormId
        );

    public delegate void OnAttack
        (
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetActorMarshaler))]
                INetActor attacker

            , [In] NetAttackType attackType
            , [In] uint weaponFormId
            , [In] uint projectileFormId
        );
}
