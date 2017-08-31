using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace WindowTitleEx {
    delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr param);

    [SuppressUnmanagedCodeSecurity]
    static class NativeMethods {
        [DllImport("user32")]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr param);

        [DllImport("user32")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxSize);

        [DllImport("user32")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int pid);

        [DllImport("user32")]
        public static extern bool SetWindowText(IntPtr hWnd, string text);
    }
}
