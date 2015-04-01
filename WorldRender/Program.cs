using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WorldRender
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if !DEBUG
            // Hide mouse cursor in RELEASE mode
            Cursor.Hide();
#endif

            using (var instance = new GameInstance())
            {
                instance.Scene = new TestScene(instance);
                instance.Run();
            }
        }
    }
}
