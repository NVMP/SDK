
using System.Collections.Generic;

namespace NVMP
{
    /// <summary>
    /// A utility helper for GECK keyboard operations and scan codes.
    /// Kept in sync with gamefalloutnv.h.
    /// </summary>
    public static class Keyboard
    {
        /// <summary>
        /// Control codes defined by GECK.
        /// </summary>
        public enum ControlCodes : uint
        {
		    Forward   = 0,
		    Backward  = 1,
		    Left      = 2,
		    Right     = 3,
		    Attack    = 4,
		    Activate  = 5,
		    Block     = 6,
		    ReadyItem = 7,
		    Crouch    = 8,
		    Run       = 9,
		    AlwaysRun = 10,
		    AutoMove  = 11,
		    Jump      = 12,
		    TogglePOV = 13,
		    MenuMode  = 14,
		    Rest      = 15,
		    VATS      = 16,
		    Hotkey1   = 17,
		    Hotkey2   = 18,
		    Hotkey3   = 19,
		    Hotkey4   = 20,
		    Hotkey5   = 21,
		    Hotkey6   = 22,
		    Hotkey7   = 23,
		    Hotkey8   = 24,
		    Quicksave = 25,
		    Quickload = 26,
		    Grab      = 27
        }

        /// <summary>
        /// Scancodes defined by DirectX / GECK.
        /// </summary>
        public enum ScanCodes : uint
        {
            // Mouse Input Specific
            Mouse_Primary   = 0,
            Mouse_Secondary = 1,

            // Keyboard Input Specific
            Key_Escape = 1,
            Key_1 = 2,
            Key_2 = 3,
            Key_3 = 4,
            Key_4 = 5,
            Key_5 = 6,
            Key_6 = 7,
            Key_7 = 8,
            Key_8 = 9,
            Key_9 = 10,
            Key_0 = 11,

            Key_Minus = 12,
            Key_Equals = 13,
            Key_Backspace = 14,
            Key_Tab = 15,

            Key_Q = 16,
            Key_W = 17,
            Key_E = 18,
            Key_R = 19,
            Key_T = 20,
            Key_Y = 21,
            Key_U = 22,
            Key_I = 23,
            Key_O = 24,
            Key_P = 25,

            Key_LeftBracket = 26,
            Key_RightBracket = 27,
            Key_Enter = 28,
            Key_LeftControl = 29,

            Key_A = 30,
            Key_S = 31,
            Key_D = 32,
            Key_F = 33,
            Key_G = 34,
            Key_H = 35,
            Key_J = 36,
            Key_K = 37,
            Key_L = 38,

            Key_Semicolon = 39,
            Key_Apostrophe = 40,
            Key_Tilde = 41,
            Key_LeftShift = 42,
            Key_BackSlash = 43,

            Key_Z = 44,
            Key_X = 45,
            Key_C = 46,
            Key_V = 47,
            Key_B = 48,
            Key_N = 49,
            Key_M = 50,

            Key_Comma = 51,
            Key_Period = 52,
            Key_ForwardSlash = 53,
            Key_RightShift = 54,
            Key_Numpad_Star = 55,
            Key_LeftAlt = 56,
            Key_Spacebar = 57,
            Key_CapsLock = 58,

            Key_F1 = 59,
            Key_F2 = 60,
            Key_F3 = 61,
            Key_F4 = 62,
            Key_F5 = 63,
            Key_F6 = 64,
            Key_F7 = 65,
            Key_F8 = 66,
            Key_F9 = 67,
            Key_F10 = 68,

            Key_NumLock = 69,
            Key_ScrollLock = 70,

            Key_NumPad_7 = 71,
            Key_NumPad_8 = 72,
            Key_NumPad_9 = 73,
            Key_NumPad_Minus = 74,
            Key_NumPad_4 = 75,
            Key_NumPad_5 = 76,
            Key_NumPad_6 = 77,
            Key_NumPad_Plus = 78,
            Key_NumPad_1 = 79,
            Key_NumPad_2 = 80,
            Key_NumPad_3 = 81,
            Key_NumPad_0 = 82,
            Key_NumPad_Period = 83,

            Key_F11 = 87,
            Key_F12 = 88,
        }
    }
}
