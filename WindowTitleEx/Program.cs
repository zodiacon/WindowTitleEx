using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowTitleEx.Properties;

namespace WindowTitleEx {
    static class Program {
        static Mutex _singleInstanceMutex;

        [STAThread]
        static void Main() {
            bool isNew;
            _singleInstanceMutex = new Mutex(true, "WindowTitleExSingleInstanceMutex", out isNew);
            if (!isNew)
                return;

            var manager = new WindowTitleManager();
            manager.Run();
        }
    }
}
