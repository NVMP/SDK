using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NVMP.Entities.GUI
{
    public class GUIWindowTemplateBuilder
    {
        internal class GUIWindowTemplate : IGUIWindowTemplate
        {
            #region Natives
            //
            // Lifetime Control
            //
            [DllImport("Native", EntryPoint = "GUI_ComposePresentMessage")]
            internal static extern IntPtr Internal_GUIComposePresentMessage();

            [DllImport("Native", EntryPoint = "GUI_DestroyPresentMessage")]
            internal static extern void Internal_GUIDestroyPresentMessage(IntPtr message);

            //
            // Window Configuration
            //
            [DllImport("Native", EntryPoint = "GUI_Window_SetID")]
            internal static extern void Internal_GUI_Window_SetID(IntPtr message, uint id);
            [DllImport("Native", EntryPoint = "GUI_Window_SetTitle")]
            internal static extern void Internal_GUI_Window_SetTitle(IntPtr message, string title);
            [DllImport("Native", EntryPoint = "GUI_Window_SetImGuiFlags")]
            internal static extern void Internal_GUI_Window_SetImGuiFlags(IntPtr message, uint flags);
            [DllImport("Native", EntryPoint = "GUI_Window_SetCanBeClosed")]
            internal static extern void Internal_GUI_Window_SetCanBeClosed(IntPtr message, bool canBeClosed);
            [DllImport("Native", EntryPoint = "GUI_Window_SetRequiresInputFocus")]
            internal static extern void Internal_GUI_Window_SetRequiresInputFocus(IntPtr message, bool requiredInputFocus);
            [DllImport("Native", EntryPoint = "GUI_Window_SetPosition")]
            internal static extern void Internal_GUI_Window_SetPosition(IntPtr message, float x, float y);
            [DllImport("Native", EntryPoint = "GUI_Window_SetDimensions")]
            internal static extern void Internal_GUI_Window_SetDimensions(IntPtr message, float width, float height);

            [DllImport("Native", EntryPoint = "GUI_Window_PresentToPlayer")]
            internal static extern void Internal_GUI_Window_PresentToPlayer(IntPtr message, IntPtr playerPtr);

            //
            // Element Modification
            //
            [DllImport("Native", EntryPoint = "GUI_Window_AddElement")]
            internal static extern IntPtr Internal_GUI_Window_AddElement(IntPtr message);
            #endregion

            public uint ID { get; internal set; }

            public string Title { get; internal set; }

            public ImGuiWindowFlags ImGuiFlags { get; internal set; }

            public bool CanBeClosed { get; internal set; }

            public bool RequiresInputFocus { get; internal set; }

            public Vector2 Position { get; internal set; }

            public Vector2 Dimensions { get; internal set; }

            public IList<IGUIBaseElement> Elements { get; internal set; } = new List<IGUIBaseElement>();

            internal uint NumTotalElements { get; set; }

            internal void PresentToPlayer(INetPlayer player)
            {
                // create the network message to send and handle it in the managed environment
                var nativeMessage = Internal_GUIComposePresentMessage();
                if (nativeMessage == IntPtr.Zero)
                    return;

                try
                {
                    // configure the window
                    Internal_GUI_Window_SetID(nativeMessage, ID);
                    Internal_GUI_Window_SetTitle(nativeMessage, Title);
                    Internal_GUI_Window_SetImGuiFlags(nativeMessage, (uint)ImGuiFlags);
                    Internal_GUI_Window_SetCanBeClosed(nativeMessage, CanBeClosed);
                    Internal_GUI_Window_SetCanBeClosed(nativeMessage, CanBeClosed);
                    Internal_GUI_Window_SetPosition(nativeMessage, Position.X, Position.Y);
                    Internal_GUI_Window_SetDimensions(nativeMessage, Dimensions.X, Dimensions.Y);

                    // configure elements - root items - this then does children underneath
                    foreach (var element in Elements)
                    {
                        var nativeElement = Internal_GUI_Window_AddElement(nativeMessage);
                        element.ConfigureNative(nativeElement);
                    }

                    Internal_GUI_Window_PresentToPlayer(nativeMessage, (player as NetPlayer).__UnmanagedAddress);
                }
                finally
                {
                    // always destroy network messages
                    Internal_GUIDestroyPresentMessage(nativeMessage);
                }
                // prepare the window information on the native side

                // populate all elements and sub elements
                // - for delegates, pass on a weak reference to the native side to call. if this window template goes out of context
                //   then we'll just silently ignore / remove the handle on the native side, but it'll still try to call into this for notifications
            }

            internal IGUIBaseElement SearchElementListForID(IList<IGUIBaseElement> elements, ulong id)
            {
                foreach (var element in elements)
                {
                    if (element.ID == id)
                    {
                        return element;
                    }

                    if (element.Children != null)
                    {
                        return SearchElementListForID(element.Children, id);
                    }
                }

                return null;
            }

            public IGUIBaseElement FindElementByID(ulong id)
            {
                return SearchElementListForID(Elements, id);
            }
        }

        internal static uint NextWindowID = 0xFF000000;
        internal GUIWindowTemplate Instance = new GUIWindowTemplate { ID = ++NextWindowID };

        /// <summary>
        /// Adds a title to this window template.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public GUIWindowTemplateBuilder WithTitle(string title)
        {
            Instance.Title = title;
            return this;
        }

        /// <summary>
        /// Marks that this window can be manually closed by the player via the menu bar.
        /// </summary>
        /// <returns></returns>
        public GUIWindowTemplateBuilder AsCanBeClosed()
        {
            Instance.CanBeClosed = true;
            return this;
        }

        /// <summary>
        /// Marks that this window requires input focus on creation, instead of only being (potentially) interactable by gaining
        /// game chat focus. This will lock the player's input until the window is dismissed, so be aware of that.
        /// </summary>
        /// <returns></returns>
        public GUIWindowTemplateBuilder AsRequiresInputFocus()
        {
            Instance.RequiresInputFocus = true;
            return this;
        }

        public GUIWindowTemplateBuilder WithPosition(Vector2 pos)
        {
            Instance.Position = pos;
            return this;
        }

        public GUIWindowTemplateBuilder WithDimensions(Vector2 size)
        {
            Instance.Dimensions = size;
            return this;
        }

        public GUIWindowTemplateBuilder WithFlags(ImGuiWindowFlags flags)
        {
            Instance.ImGuiFlags = flags;
            return this;
        }

        public GUIWindowTemplateBuilder WithElements(Action<GUIWindowElementBuilder> builder)
        {
            builder(new GUIWindowElementBuilder(Instance.Elements, Instance));
            return this;
        }

        public IGUIWindowTemplate Build()
        {
            var inst = Instance;
            Instance = null;
            return inst;
        }

    }
}
