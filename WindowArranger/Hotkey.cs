using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace WindowArranger
{
    public class Hotkey : IMessageFilter
    {
        #region Interop

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        private const uint WM_HOTKEY = 0x312;

        private const uint MOD_ALT = 0x1;
        private const uint MOD_CONTROL = 0x2;
        private const uint MOD_SHIFT = 0x4;
        private const uint MOD_WIN = 0x8;

        private const uint ERROR_HOTKEY_ALREADY_REGISTERED = 1409;

        #endregion

        private static int currentID;
        private const int maximumID = 0xBFFF;

        private Keys keyCode;
        private bool shift;
        private bool control;
        private bool alt;
        private bool windows;

        [XmlIgnore]
        private int id;
        [XmlIgnore]
        private bool registered;
        [XmlIgnore]
        private Control windowControl;

        public event HandledEventHandler Pressed;

        public Hotkey() : this(Keys.None, false, false, false, false)
        {
        }

        public Hotkey(Keys keyCode, bool shift, bool control, bool alt, bool windows)
        {
            this.KeyCode = keyCode;
            this.Shift = shift;
            this.Control = control;
            this.Alt = alt;
            this.Windows = windows;

            Application.AddMessageFilter(this);
        }

        ~Hotkey()
        {
            if (this.Registered)
            {
                this.Unregister();
            }
        }

        public Hotkey Clone()
        {
            return new Hotkey(this.keyCode, this.shift, this.control, this.alt, this.windows);
        }

        public bool GetCanRegister(Control windowControl)
        {
            try
            {
                if (!this.Register(windowControl))
                {
                    return false;
                }

                this.Unregister();
                return true;
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        public bool Register(Control windowControl)
        {
            if (this.registered)
            {
                throw new NotSupportedException("You cannot register a hotkey that is already registered");
            }

            if (this.Empty)
            {
                throw new NotSupportedException("You cannot register an empty hotkey");
            }

            this.id = Hotkey.currentID;
            Hotkey.currentID = Hotkey.currentID + 1 % Hotkey.maximumID;

            uint modifiers = (this.Alt ? Hotkey.MOD_ALT : 0) | (this.Control ? Hotkey.MOD_CONTROL : 0) |
                            (this.Shift ? Hotkey.MOD_SHIFT : 0) | (this.Windows ? Hotkey.MOD_WIN : 0);

            if (Hotkey.RegisterHotKey(windowControl.Handle, this.id, modifiers, keyCode) == 0)
            {
                if (Marshal.GetLastWin32Error() == ERROR_HOTKEY_ALREADY_REGISTERED)
                {
                    return false;
                }
                else
                {
                    throw new Win32Exception();
                }
            }

            this.registered = true;
            this.windowControl = windowControl;

            return true;
        }

        public void Unregister()
        {
            if (!this.registered)
            {
                throw new NotSupportedException("You cannot unregister a hotkey that is not registered");
            }

            if (!this.windowControl.IsDisposed)
            {
                if (Hotkey.UnregisterHotKey(this.windowControl.Handle, this.id) == 0)
                {
                    throw new Win32Exception();
                }
            }

            this.registered = false;
            this.windowControl = null;
        }

        private void Reregister()
        {
            if (!this.registered)
            {
                return;
            }

            Control windowControl = this.windowControl;

            this.Unregister();
            this.Register(windowControl);
        }

        public bool PreFilterMessage(ref Message message)
        {
            if (message.Msg != Hotkey.WM_HOTKEY)
            {
                return false;
            }

            if (this.registered && (message.WParam.ToInt32() == this.id))
            {
                return this.OnPressed();
            }
            else
            {
                return false;
            }
        }

        private bool OnPressed()
        {
            HandledEventArgs handledEventArgs = new HandledEventArgs(false);
            if (this.Pressed != null)
            {
                this.Pressed(this, handledEventArgs);
            }

            return handledEventArgs.Handled;
        }

        public override string ToString()
        {
            if (this.Empty)
            {
                return "(none)";
            }

            string keyName = Enum.GetName(typeof(Keys), this.keyCode); ;
            switch (this.keyCode)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    keyName = keyName.Substring(1);
                    break;
                default:
                    break;
            }

            string modifiers = "";
            if (this.shift)
            {
                modifiers += "Shift+";
            }
            if (this.control)
            {
                modifiers += "Control+";
            }
            if (this.alt)
            {
                modifiers += "Alt+";
            }
            if (this.windows)
            {
                modifiers += "Windows+";
            }

            return modifiers + keyName;
        }

        public bool Empty
        {
            get { return this.keyCode == Keys.None; }
        }

        public bool Registered
        {
            get { return this.registered; }
        }

        public Keys KeyCode
        {
            get { return this.keyCode; }
            set
            {
                this.keyCode = value;
                this.Reregister();
            }
        }

        public bool Shift
        {
            get { return this.shift; }
            set
            {
                this.shift = value;
                this.Reregister();
            }
        }

        public bool Control
        {
            get { return this.control; }
            set
            {
                this.control = value;
                this.Reregister();
            }
        }

        public bool Alt
        {
            get { return this.alt; }
            set
            {
                this.alt = value;
                this.Reregister();
            }
        }

        public bool Windows
        {
            get { return this.windows; }
            set
            {
                this.windows = value;
                this.Reregister();
            }
        }
    }
}
