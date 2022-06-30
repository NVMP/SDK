using NVMP.Entities;
using System.Runtime.InteropServices;

namespace NVMP.Delegates
{
    // Function pointer delegate for program update loops
    public delegate void UpdateDelegate(float delta);
    public delegate void PlayerJoinedDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
    );

    public delegate void PlayerCheatedDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
    );

    public delegate void PlayerLeftDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
    );

    public delegate void PlayerRequestsPreJoinDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
        ,
        [In, MarshalAs(UnmanagedType.LPStr)] string token
    );

    public delegate void PlayerRequestsRespawnDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
    );

    public delegate bool PlayerCommandDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
        ,

        [In, MarshalAs(UnmanagedType.LPWStr)] string commandName,
        [In] uint numParams,
        [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)] string[] paramList
    );

    public delegate void PlayerMessageDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
        ,

        [In, MarshalAs(UnmanagedType.LPWStr)] string message
    );

    public delegate bool CanResendChatToDelegate
    (
        // 
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
        ,

        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer target
        ,

        [In, Out, MarshalAs(UnmanagedType.LPWStr)] string message
        ,
        [In, Out, MarshalAs(UnmanagedType.LPWStr)] ref string username
        ,
        [In, Out, MarshalAs(UnmanagedType.U1)] ref byte UCA
        ,
        [In, Out, MarshalAs(UnmanagedType.U1)] ref byte UCR
        ,
        [In, Out, MarshalAs(UnmanagedType.U1)] ref byte UCG
        ,
        [In, Out, MarshalAs(UnmanagedType.U1)] ref byte UCB
    );

    public delegate bool CanCharacterChangeNameDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetActorMarshaler))]
                INetActor character
    );

    public delegate bool CanResendVoiceToDelegate
    (
        // 
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
        ,

        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer target
        ,
        [In] ref float volume
    );

    public delegate void ActorDiedDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetActorMarshaler))]
                INetActor actor
        ,

        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetActorMarshaler))]
                INetActor victim
    );

    public delegate void PlayerUpdatedSaveDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
        ,

        [In, MarshalAs(UnmanagedType.LPStr)] string savename

        ,
        [In] string digest
    );

    public delegate void PlayerSaveEventDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player
    );

    public delegate void InputUpdateDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player,

        [In] uint inputType,
        [In] uint key
    );

    public delegate void MouseUpdateDelegate
    (
        [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshals.NetPlayerMarshaler))]
                NetPlayer player,

        [In] int mouseX,
        [In] int mouseY,
        [In] int mousewheelZ
    );

}
