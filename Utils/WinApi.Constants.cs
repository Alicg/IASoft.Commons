using System;

namespace Utils
{
    partial class WinApi
    {
        #region - WindowMessages -

        public const int WM_SETREDRAW = 0x000B;

        public const int WM_PAINT = 0x000F;

        public const int WM_NCHITTEST = 0x0084;
        public const int WM_SETFOCUS = 0x0007;


        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
        /// </summary>
        public const int WM_MOUSEMOVE = 0x200;

        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
        /// </summary>
        public const int WM_LBUTTONDOWN = 0x201;

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        public const int WM_RBUTTONDOWN = 0x204;

        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
        /// </summary>
        public const int WM_MBUTTONDOWN = 0x207;

        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
        /// </summary>
        public const int WM_LBUTTONUP = 0x202;

        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
        /// </summary>
        public const int WM_RBUTTONUP = 0x205;

        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
        /// </summary>
        public const int WM_MBUTTONUP = 0x208;

        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
        /// </summary>
        public const int WM_LBUTTONDBLCLK = 0x203;

        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
        /// </summary>
        public const int WM_RBUTTONDBLCLK = 0x206;

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
        /// </summary>
        public const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        /// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
        /// </summary>
        public const int WM_MOUSEWHEEL = 0x020A;

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
        /// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        public const int WM_KEYDOWN = 0x100;

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
        /// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
        /// or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        public const int WM_KEYUP = 0x101;

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
        /// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
        /// presses another key. It also occurs when no window currently has the keyboard focus; 
        /// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
        /// receives the message can distinguish between these two contexts by checking the context 
        /// code in the lParam parameter. 
        /// </summary>
        public const int WM_SYSKEYDOWN = 0x104;

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
        /// releases a key that was pressed while the ALT key was held down. It also occurs when no 
        /// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
        /// to the active window. The window that receives the message can distinguish between 
        /// these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        public const int WM_SYSKEYUP = 0x105;

        public const int WM_TIMER = 0x0113;

        public const uint WM_FONTCHANGE = 0x1D;

        public static IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        #endregion

        #region - HitTest Results -

        public const int HTTRANSPARENT = -1;
        public const int HTBORDER = 18;
        public const int HTCAPTION = 2;

        #endregion

        #region - WindowStyle Codes -

        public const int WS_EX_TRANSPARENT = 0x00000020;

        // offset of window style value
        public const int GWL_STYLE = -16;

        // window style constants for scrollbars
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;

        #endregion

        #region - WindowHook Codes -

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        public const int WH_MOUSE_LL = 14;

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        public const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
        /// </summary>
        public const int WH_MOUSE = 7;

        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
        /// </summary>
        public const int WH_KEYBOARD = 2;

        #endregion

        #region - VirtualKey Codes -

        public const byte VK_LBUTTON = 0x01; //Left mouse button
        public const byte VK_RBUTTON = 0x02; //Right mouse button
        public const byte VK_CANCEL = 0x03; //Control-break processing
        public const byte VK_MBUTTON = 0x04; //Middle mouse button (three-button mouse)
        public const byte VK_BACK = 0x08; //BACKSPACE key
        public const byte VK_TAB = 0x09; //TAB key
        public const byte VK_CLEAR = 0x0C; //CLEAR key
        public const byte VK_RETURN = 0x0D; //ENTER key
        public const byte VK_SHIFT = 0x10; //SHIFT key
        public const byte VK_CONTROL = 0x11; //CTRL key
        public const byte VK_MENU = 0x12; //ALT key
        public const byte VK_PAUSE = 0x13; //PAUSE key
        public const byte VK_CAPITAL = 0x14; //CAPS LOCK key
        public const byte VK_ESCAPE = 0x1B; //ESC key
        public const byte VK_SPACE = 0x20; //SPACEBAR
        public const byte VK_PRIOR = 0x21; //PAGE UP key
        public const byte VK_NEXT = 0x22; //PAGE DOWN key
        public const byte VK_END = 0x23; //END key
        public const byte VK_HOME = 0x24; //HOME key
        public const byte VK_LEFT = 0x25; //LEFT ARROW key
        public const byte VK_UP = 0x26; //UP ARROW key
        public const byte VK_RIGHT = 0x27; //RIGHT ARROW key
        public const byte VK_DOWN = 0x28; //DOWN ARROW key
        public const byte VK_SELECT = 0x29; //SELECT key
        public const byte VK_PRINT = 0x2A; //PRINT key
        public const byte VK_EXECUTE = 0x2B; //EXECUTE key
        public const byte VK_SNAPSHOT = 0x2C; //PRINT SCREEN key
        public const byte VK_INSERT = 0x2D; //INS key
        public const byte VK_DELETE = 0x2E; //DEL key
        public const byte VK_HELP = 0x2F; //HELP key
        public const byte VK_0 = 0x30; //0 key
        public const byte VK_1 = 0x31; //1 key
        public const byte VK_2 = 0x32; //2 key
        public const byte VK_3 = 0x33; //3 key
        public const byte VK_4 = 0x34; //4 key
        public const byte VK_5 = 0x35; //5 key
        public const byte VK_6 = 0x36; //6 key
        public const byte VK_7 = 0x37; //7 key
        public const byte VK_8 = 0x38; //8 key
        public const byte VK_9 = 0x39; //9 key
        public const byte VK_A = 0x41; //A key
        public const byte VK_B = 0x42; //B key
        public const byte VK_C = 0x43; //C key
        public const byte VK_D = 0x44; //D key
        public const byte VK_E = 0x45; //E key
        public const byte VK_F = 0x46; //F key
        public const byte VK_G = 0x47; //G key
        public const byte VK_H = 0x48; //H key
        public const byte VK_I = 0x49; //I key
        public const byte VK_J = 0x4A; //J key
        public const byte VK_K = 0x4B; //K key
        public const byte VK_L = 0x4C; //L key
        public const byte VK_M = 0x4D; //M key
        public const byte VK_N = 0x4E; //N key
        public const byte VK_O = 0x4F; //O key
        public const byte VK_P = 0x50; //P key
        public const byte VK_Q = 0x51; //Q key
        public const byte VK_R = 0x52; //R key
        public const byte VK_S = 0x53; //S key
        public const byte VK_T = 0x54; //T key
        public const byte VK_U = 0x55; //U key
        public const byte VK_V = 0x56; //V key
        public const byte VK_W = 0x57; //W key
        public const byte VK_X = 0x58; //X key
        public const byte VK_Y = 0x59; //Y key
        public const byte VK_Z = 0x5A; //Z key
        public const byte VK_NUMPAD0 = 0x60; //Numeric keypad 0 key
        public const byte VK_NUMPAD1 = 0x61; //Numeric keypad 1 key
        public const byte VK_NUMPAD2 = 0x62; //Numeric keypad 2 key
        public const byte VK_NUMPAD3 = 0x63; //Numeric keypad 3 key
        public const byte VK_NUMPAD4 = 0x64; //Numeric keypad 4 key
        public const byte VK_NUMPAD5 = 0x65; //Numeric keypad 5 key
        public const byte VK_NUMPAD6 = 0x66; //Numeric keypad 6 key
        public const byte VK_NUMPAD7 = 0x67; //Numeric keypad 7 key
        public const byte VK_NUMPAD8 = 0x68; //Numeric keypad 8 key
        public const byte VK_NUMPAD9 = 0x69; //Numeric keypad 9 key
        public const byte VK_SEPARATOR = 0x6C; //Separator key
        public const byte VK_SUBTRACT = 0x6D; //Subtract key
        public const byte VK_DECIMAL = 0x6E; //Decimal key
        public const byte VK_DIVIDE = 0x6F; //Divide key
        public const byte VK_F1 = 0x70; //F1 key
        public const byte VK_F2 = 0x71; //F2 key
        public const byte VK_F3 = 0x72; //F3 key
        public const byte VK_F4 = 0x73; //F4 key
        public const byte VK_F5 = 0x74; //F5 key
        public const byte VK_F6 = 0x75; //F6 key
        public const byte VK_F7 = 0x76; //F7 key
        public const byte VK_F8 = 0x77; //F8 key
        public const byte VK_F9 = 0x78; //F9 key
        public const byte VK_F10 = 0x79; //F10 key
        public const byte VK_F11 = 0x7A; //F11 key
        public const byte VK_F12 = 0x7B; //F12 key
        public const byte VK_SCROLL = 0x91; //SCROLL LOCK key
        public const byte VK_LSHIFT = 0xA0; //Left SHIFT key
        public const byte VK_RSHIFT = 0xA1; //Right SHIFT key
        public const byte VK_LCONTROL = 0xA2; //Left CONTROL key
        public const byte VK_RCONTROL = 0xA3; //Right CONTROL key
        public const byte VK_LMENU = 0xA4; //Left MENU key
        public const byte VK_RMENU = 0xA5; //Right MENU key
        public const byte VK_PLAY = 0xFA; //Play key
        public const byte VK_ZOOM = 0xFB; //Zoom key

        #endregion
    }
}