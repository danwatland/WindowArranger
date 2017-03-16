using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsArranger;
using static WindowArranger.NativeMethods;

namespace WindowArranger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Taskbar = new Taskbar();
            var form = new BackgroundForm();
            var hotkeyList = new List<Hotkey>()
            {
                RegisterKey(form, WindowPosition.LOWER_LEFT),
                RegisterKey(form, WindowPosition.LOWER_RIGHT),
                RegisterKey(form, WindowPosition.UPPER_LEFT),
                RegisterKey(form, WindowPosition.UPPER_RIGHT),
            };

            Application.Run(form);
            hotkeyList.ForEach(h => h.Unregister());
        }

        static Taskbar Taskbar;

        static Hotkey RegisterKey(Control form, WindowPosition position)
        {
            Hotkey hk = new Hotkey();
            hk.KeyCode = (Keys)position;
            hk.Windows = true;
            hk.Control = true;
            hk.Pressed += delegate { MoveWindow(position); };
            hk.Register(form);

            return hk;
        }

        static void MoveWindow(WindowPosition position)
        {
            var windowPtr = NativeMethods.GetForegroundWindow();
            var monitorInfo = new NativeMonitorInfo();

            var monitor = NativeMethods.MonitorFromWindow(windowPtr, NativeMethods.MONITOR_DEFAULTTONEAREST);
            NativeMethods.GetMonitorInfo(monitor, monitorInfo);

            var width = monitorInfo.Monitor.Right - monitorInfo.Monitor.Left;
            var height = monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top - Taskbar.Bounds.Height;

            NativeMethods.NormalizeWindow(windowPtr);

            switch (position)
            {
                case WindowPosition.UPPER_LEFT:
                    NativeMethods.MoveWindow(windowPtr, monitorInfo.Monitor.Left, monitorInfo.Monitor.Top, width / 2, height / 2, true);
                    break;
                case WindowPosition.LOWER_LEFT:
                    NativeMethods.MoveWindow(windowPtr, monitorInfo.Monitor.Left, monitorInfo.Monitor.Top + height / 2, width / 2, height / 2, true);
                    break;
                case WindowPosition.UPPER_RIGHT:
                    NativeMethods.MoveWindow(windowPtr, monitorInfo.Monitor.Left + width / 2, monitorInfo.Monitor.Top, width / 2, height / 2, true);
                    break;
                case WindowPosition.LOWER_RIGHT:
                    NativeMethods.MoveWindow(windowPtr, monitorInfo.Monitor.Left + width / 2, monitorInfo.Monitor.Top + height / 2, width / 2, height / 2, true);
                    break;
                default:
                    break;
            }
        }
    }
}
