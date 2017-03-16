using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowArranger
{
    public static class NativeMethods
    {
        public const Int32 MONITOR_DEFAULTTOPRIMERTY = 0x00000001;
        public const Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const Int32 SW_SHOWNORMAL = 1;

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);

        [DllImport("user32.dll")]
        public static extern Boolean GetMonitorInfo(IntPtr hMonitor, NativeMonitorInfo lpmi);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        public static bool NormalizeWindow(IntPtr hWnd)
        {
            return ShowWindowAsync(hWnd, SW_SHOWNORMAL);
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct NativeRectangle
        {
            public Int32 Left;
            public Int32 Top;
            public Int32 Right;
            public Int32 Bottom;


            public NativeRectangle(Int32 left, Int32 top, Int32 right, Int32 bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public sealed class NativeMonitorInfo
        {
            public Int32 Size = Marshal.SizeOf(typeof(NativeMonitorInfo));
            public NativeRectangle Monitor;
            public NativeRectangle Work;
            public Int32 Flags;
        }
    }
}
