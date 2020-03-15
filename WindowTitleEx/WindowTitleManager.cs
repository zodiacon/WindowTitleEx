using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowTitleEx.Properties;

namespace WindowTitleEx {
	class WindowTitleManager {
		NotifyIcon _tray;
		List<string> _excludeProcesses = new List<string>(4);
		HashSet<int> _excludePids = new HashSet<int>();
		string _exlcudeProcessesText;

		public void Run() {
			_excludePids.Add(Process.GetCurrentProcess().Id);
			var revertTitlesAndCloseMenuItem = new MenuItem {
				Text = "Revert Titles and Close",
			};
			var closeMenuItem = new MenuItem {
				Text = "Close"
			};
			var settingsMenu = new MenuItem {
				Text = "Settings...",
				DefaultItem = true
			};
			var menu = new ContextMenu {
				MenuItems = {
					revertTitlesAndCloseMenuItem, closeMenuItem,
					new MenuItem { Text = "-" },
					settingsMenu
				}
			};

			revertTitlesAndCloseMenuItem.Click += OnRevertAndClose;
			closeMenuItem.Click += OnClose;
			settingsMenu.Click += OnSettings;

			_tray = new NotifyIcon {
				ContextMenu = menu,
				Text = "Window Title Ex",
				Icon = Resources.app,
				Visible = true,
			};

			_tray.MouseDoubleClick += (s, e) => OnSettings(s, e);

			_timer = new System.Windows.Forms.Timer {
				Interval = 3000,
				Enabled = true
			};
			_timer.Tick += OnTick;
			Application.Run();
		}

		private void OnSettings(object sender, EventArgs e) {
			using (var form = new SettingsForm(_exlcudeProcessesText)) {
				if (form.ShowDialog() == DialogResult.OK) {
					_exlcudeProcessesText = form.ExcludedProcessesText;

					_excludeProcesses = _exlcudeProcessesText.Split(new[] { '\n', '\r' }, 
						StringSplitOptions.RemoveEmptyEntries).ToList();
					_excludePids.Clear();
					_excludePids.Add(Process.GetCurrentProcess().Id);
					RevertWindows();
				}
			}
		}

		void RevertWindows() {
			var text = new StringBuilder(512);
			NativeMethods.EnumWindows((hWnd, p) => {
				if (NativeMethods.IsWindowVisible(hWnd)) {
					NativeMethods.GetWindowText(hWnd, text, text.Capacity);
					if (text.Length > 0) {
						var stext = text.ToString();
						int index = stext.IndexOf("(HWND: ");
						if (index >= 0) {
							stext = stext.Substring(0, index - 1);
							NativeMethods.SetWindowText(hWnd, stext);
						}
					}
				}
				return true;
			}, IntPtr.Zero);
		}

		private void OnRevertAndClose(object sender, EventArgs e) {
			try {
				_timer.Enabled = false;
				RevertWindows();
				_timer.Dispose();
				_tray.Dispose();
			}
			finally {
				Application.Exit();
			}
		}

		System.Windows.Forms.Timer _timer;

		private void OnClose(object sender, EventArgs e) {
			_timer.Dispose();
			_tray.Dispose();
			Application.Exit();
		}

		private void OnTick(object sender, EventArgs e) {
			var text = new StringBuilder(512);
			NativeMethods.EnumWindows((hWnd, p) => {
				if (NativeMethods.IsWindowVisible(hWnd)) {
					int pid;
					int tid = NativeMethods.GetWindowThreadProcessId(hWnd, out pid);
					if (_excludePids.Contains(pid))
						return true;
					if (_excludeProcesses.Count > 0) {
						var process = Process.GetProcessById(pid);
						if (_excludeProcesses.Contains(process.ProcessName.ToLower())) {
							_excludePids.Add(pid);
							return true;
						}
					}

					NativeMethods.GetWindowText(hWnd, text, text.Capacity);
					NativeMethods.GetWindowRect(hWnd, out var rc);
					if (rc.Width > 50 && rc.Height > 1 && text.Length > 0) {
						var stext = text.ToString();
						if (stext.LastIndexOf("PID: ") < 0) {
							text.Append($" (HWND: 0x{hWnd.ToInt64():X}, TID: {tid}, PID: {pid})");
							NativeMethods.SetWindowText(hWnd, text.ToString());
						}
					}
				}
				return true;
			}, IntPtr.Zero);
		}

	}
}
