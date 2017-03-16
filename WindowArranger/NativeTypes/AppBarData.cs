using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsArranger.NativeTypes
{

    [StructLayout(LayoutKind.Sequential)]
    public struct AppBarData
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public Rect rc;
        public int lParam;
    }
}
