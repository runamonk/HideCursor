using System;
using System.Windows.Forms;

namespace HideCursor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);    
            _ = new HideCursor();
            Application.Run();
        }
    }
}
