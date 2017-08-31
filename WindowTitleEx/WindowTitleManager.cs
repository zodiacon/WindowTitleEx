using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowTitleEx.Properties;

namespace WindowTitleEx {
    class WindowTitleManager {
        NotifyIcon _tray;

        public void Run() {
            _tray = new NotifyIcon();
            var menu = new ContextMenu {
                MenuItems = {
                    new MenuItem {
                        Text = "Revert Titles and Close",
                    },
                    new MenuItem {
                        Text = "Close"
                    }
                }
            };
            menu.MenuItems[0].Click += OnRevertAndClose;
            menu.MenuItems[1].Click += OnClose;
            _tray.ContextMenu = menu;
            _tray.Text = "Window Title Ex";
            _tray.Icon = Resources.app;
            _tray.Visible = true;

            _timer = new System.Windows.Forms.Timer {
                Interval = 3000,
                Enabled = true
            };
            _timer.Tick += OnTick;
            Application.Run();
        }

        private void OnRevertAndClose(object sender, EventArgs e) {
            _timer.Enabled = false;
            _timer.Dispose();
            var text = new StringBuilder(512);
            NativeMethods.EnumWindows((hWnd, p) => {
                if (NativeMethods.IsWindowVisible(hWnd)) {
                    NativeMethods.GetWindowText(hWnd, text, text.Capacity);
                    if (text.Length > 0) {
                        var stext = text.ToString();
                        int index = stext.IndexOf("(HWND: ");
                        if(index >= 0) {
                            stext = stext.Substring(0, index);
                            NativeMethods.SetWindowText(hWnd, stext);
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);
            _tray.Dispose();
            Application.Exit();
        }

        System.Windows.Forms.Timer _timer;

        private void OnClose(object sender, EventArgs e) {
            _timer.Dispose();
            _tray.Dispose();
            Application.Exit();
        }

        private static void OnTick(object sender, EventArgs e) {
            var text = new StringBuilder(512);
            NativeMethods.EnumWindows((hWnd, p) => {
                if (NativeMethods.IsWindowVisible(hWnd)) {
                    NativeMethods.GetWindowText(hWnd, text, text.Capacity);
                    if (text.Length > 0) {
                        var stext = text.ToString();
                        if (stext.LastIndexOf("PID: ") < 0) {
                            int pid;
                            int tid = NativeMethods.GetWindowThreadProcessId(hWnd, out pid);
                            text.Append($" (HWND: 0x{hWnd:X}, TID: {tid}, PID: {pid})");
                            NativeMethods.SetWindowText(hWnd, text.ToString());
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);
        }

    }
}
