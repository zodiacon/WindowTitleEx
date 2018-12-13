using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace WindowTitleEx {
    delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr param);

	struct Rect {
		public int Left, Top, Right, Bottom;

		public int Width => Right - Left;
		public int Height => Bottom - Top;
	}

    [SuppressUnmanagedCodeSecurity]
    static class NativeMethods {
		const int WM_SETTEXT = 0xc;

        [DllImport("user32")]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr param);

        [DllImport("user32")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32", CharSet= CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxSize);

        [DllImport("user32")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int pid);

		[DllImport("user32")]
		public static extern bool GetWindowRect(IntPtr hWnd, out Rect rc);

		[DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern bool SetWindowText(IntPtr hWnd, string text);

		[DllImport("user32", CharSet = CharSet.Unicode)]
		public static extern int SendMessageTimeout(IntPtr hWnd, int message, IntPtr wParam, string lparam, int timeout, out IntPtr result);
	}
}
