using NVMP.Entities;
using System.Threading.Tasks;

namespace NVMP
{
    /// <summary>
    /// Defines an NV:MP server plugin to be loaded at runtime. IPlugins are instanciated by default if they inherit.
    /// Be aware that a plugin is by default _not_ the main server plugins. Plugins run parellel to the main game server, and are designed to add new built-in logic. 
    /// If you are designing a plugin that acts as a server, then inherit IPlugin and GameServer. Else you miss out on vital implementations such as webservice and client-mod configs.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Name of the plugin
        /// </summary>
        string GetName();

        /// <summary>
        /// A default update loop of the plugin
        /// </summary>
        /// <param name="delta">time between this frame and last frame</param>
        void Update(float delta);

        /// <summary>
        /// Initialization procedure called when the plugin is instanciated. This happens on server start up, but also when a plugin is restarted (hotloaded).
        /// So make sure that any logic you do perform inside here is also handled in Shutdown so that data does not leak over a hotload.
        /// </summary>
        void Init();

        /// <summary>
        /// Shutdown procedure called when the plugin is destroyed. Can happen multiple times followed by Init() if a plugin is hotloaded.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Called for any Player joining the server
        /// </summary>
        Task PlayerJoined(NetPlayer target);

        /// <summary>
        /// Called for any Player that leaves the server
        /// </summary>
        Task PlayerLeft(NetPlayer player);

        /// <summary>
        /// Called for any Player that has just cheated on the server
        /// </summary>
        Task PlayerCheated(NetPlayer target);

        /// <summary>
        /// Called for any Player supplying an authentication token to the server. If a player is not marked as NetPlayer.Authenticated, then the server will evict them very soon.
        /// You should always see about implementing this as a proxy to IAuthenticatorInterface and calling IAuthenticatorInterface.SetupAuthentication to process this. 
        /// </summary>
        /// <param name="player">player</param>
        /// <param name="authToken">user supplied token</param>
        /// <returns></returns>
        Task PlayerAuthenticating(NetPlayer player, string authToken);

        /// <summary>
        /// Called for any input a player supplies whilst in-game (out of UI focus).
        /// </summary>
        /// <param name="player"></param>
        /// <param name="inputType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task PlayerInputUpdate(NetPlayer player, UserInterface.InputType inputType, uint key);

        /// <summary>
        /// Called for any mouse change from a player whilst in-game (out of UI focus)
        /// Every mouse property is a delta of movement, not an accurate position
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        /// <param name="mousewheelZ"></param>
        /// <returns></returns>
        Task PlayerMouseUpdate(NetPlayer player, int mouseX, int mouseY, int mousewheelZ);

        /// <summary>
        /// Called for any Player updating their save on the server.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="digest"></param>
        /// <returns></returns>
        Task PlayerUpdatedSave(NetPlayer player, string name, string digest);

        /// <summary>
        /// Called for any Player creating a new save.
        /// </summary>
        Task PlayerNewSave(NetPlayer player);

        /// <summary>
        /// Called for any Player finishing a load of TESSaveGameData
        /// </summary>
        Task PlayerFinishLoad(NetPlayer player);

        /// <summary>
        /// Called for any Player requesting a respawn. This is always called when a player passes through the game load system, however I don't recommend
        /// using this to move a player into a saved position unless you don't support updating valid saves and are doing it hard to the metal.
        /// Can be stubbed.
        /// </summary>
        Task PlayerRequestsRespawn(NetPlayer player);

        /// <summary>
        /// Called for any Actor that has died.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="killer">TODO</param>
        Task ActorDied(INetActor actor, INetActor killer);

        /// <summary>
        /// Called for any Player that sends a message to the game chat (post-CanResend)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message">message displayed</param>
        Task PlayerMessaged(NetPlayer player, string message);

        /// <summary>
        /// A conditional check that if returns false, disallows a Character from renaming themselves.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        bool CanCharacterChangeName(INetActor character);

        /// <summary>
        /// A conditional check with mutate access to message information for if a Player can send a message to another player. We don't have a concept of blocking a message
        /// outright. If you want to "mute" a player entirely, use this function and disregard the target.
        /// This function is called for every potential player to receive the message.
        /// </summary>
        /// <param name="player">sender</param>
        /// <param name="target">potential receiver</param>
        /// <param name="message">message to send</param>
        /// <param name="username">username to display, can be modified</param>
        /// <param name="usercolor">usercolor to display, can be modified</param>
        /// <returns>return FALSE to not send the message</returns>
        bool CanResendChatTo(NetPlayer player, NetPlayer target, string message, ref string username, ref System.Drawing.Color usercolor);

        /// <summary>
        /// A conditional check with mutate access to the volume to deliver when a Player sends voice information. Voice packets are sent Player-To-Server-To-Player, and this
        /// check is always hit for every packet. So don't make it intense!
        /// </summary>
        /// <param name="player">sender</param>
        /// <param name="target">potential receiver</param>
        /// <param name="volume">volume of voice</param>
        /// <returns>return FALSE to not send the packet</returns>
        bool CanResendVoiceTo(NetPlayer player, NetPlayer target, ref float volume);

        /// <summary>
        /// Called for any Player that executes a desired command in chat using "/".
        /// </summary>
        /// <param name="player">sender</param>
        /// <param name="commandName">primary command name</param>
        /// <param name="numParams">num params in use</param>
        /// <param name="paramList">param list</param>
        /// <returns>return TRUE to consume the message from being sent to other players, FALSE to not consume</returns>
        bool PlayerExecutedCommand(NetPlayer player, string commandName, uint numParams, string[] paramList);
    }
}
