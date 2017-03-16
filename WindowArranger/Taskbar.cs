using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsArranger.NativeTypes;

namespace WindowsArranger
{
    public class Taskbar
    {
        private const int AUTOHIDE = 0x0000001;
        private const int ALWAYS_ON_TOP = 0x0000002;
        private const int GET_TASKBAR_POS = 0x0000005;
        private const int GET_STATE = 0x0000004;
        private const string TASKBAR_CLASS_NAME = "Shell_TrayWnd";

        public Taskbar()
        {
            var taskbarHandle = FindWindow(TASKBAR_CLASS_NAME, null);

            var data = new AppBarData()
            {
                cbSize = (uint)Marshal.SizeOf(typeof(AppBarData)),
                hWnd = taskbarHandle
            };

            var result = SHAppBarMessage(GET_TASKBAR_POS, ref data);
            if (result == IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }

            Bounds = Rectangle.FromLTRB(data.rc.Left, data.rc.Top, data.rc.Right, data.rc.Bottom);
            //data.cbSize = (uint)Marshal.SizeOf(typeof(AppBarData));
            //result = SHAppBarMessage(GET_STATE, ref data);
            //int state
        }

        public Rectangle Bounds { get; private set; }

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr SHAppBarMessage(uint dwMessage, [In] ref AppBarData pData);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
