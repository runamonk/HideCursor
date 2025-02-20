using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Utility;

namespace HideCursor
{
    internal class HideCursor
    {
        private NotifyIcon notifyIcon = new NotifyIcon();
        private ContextMenu contextMenu = new ContextMenu();

        private LowLevelHooks hooks = new LowLevelHooks();
        private int savedCursorY = 0;
        private int savedCursorX = 0;
        private bool cursorIsHidden = false;
        private string[] cursors = { "32512", "32513", "32514", "32515", "32516", "32642", "32643", "32644", "32645", "32646", "32648", "32649", "32650" };
        private const string activeChars = "abcdefghijklmnopqrstuvwxyz0123456789!£$~¬`{}[],.<>/?_+-=";
        private System.Windows.Forms.Timer MouseTimer;

        [DllImport("user32.dll")]
        private static extern bool SetSystemCursor(IntPtr hcur, uint id);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateCursor(IntPtr hInst, int xHotSpot, int yHotSpot, int nWidth, int nHeight, byte[] pvANDPlane, byte[] pvXORPlane);

        [DllImport("user32.dll")]
        private static extern IntPtr CopyImage(IntPtr h, uint type, int cx, int cy, uint flags);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

        private void OnKeyPressed(Keys key)
        {
            if (cursorIsHidden || (activeChars.IndexOf(key.ToString().ToLower()) == -1))
                return;
           
            cursorIsHidden = true;
            savedCursorX = Cursor.Position.X;
            savedCursorY = Cursor.Position.Y;

            foreach (string c in cursors)
            {
                SetSystemCursor(LoadCursorFromFile(@Path.GetDirectoryName(Application.ExecutablePath) + "\\blank_cursor.cur"), Convert.ToUInt32(c));
            }         
           MouseTimer.Start();
        }

        public HideCursor()
        {
            notifyIcon.Visible = true;
            notifyIcon.Icon = new System.Drawing.Icon(@Path.GetDirectoryName(Application.ExecutablePath) + "\\hide.ico");
            notifyIcon.ContextMenu = contextMenu;
            MenuItem menuExit = new MenuItem("Exit");
            menuExit.Click += (sender, args) =>
            {
                hooks.Stop();
                ResetCursor();
                Application.ExitThread();
            };
            contextMenu.MenuItems.Add(menuExit);

            MouseTimer = new System.Windows.Forms.Timer();
            MouseTimer.Interval = 250;
            MouseTimer.Tick += (sender, args) =>
            {
                if (Cursor.Position.X != savedCursorX || Cursor.Position.Y != savedCursorY)
                    ResetCursor();            
            };

            ResetCursor();
            hooks.OnKeyPress += OnKeyPressed;
            hooks.Start();
        }
        ~HideCursor()
        {
            ResetCursor();
        }

        private void ResetCursor()
        {
            MouseTimer.Stop();
            savedCursorX = Cursor.Position.X;
            savedCursorY = Cursor.Position.Y;
            cursorIsHidden = false;
            SystemParametersInfo(0x0057, 0, null, 0);
        }

    }
}
