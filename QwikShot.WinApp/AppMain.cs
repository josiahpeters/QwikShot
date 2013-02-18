using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QwikShot.WinApp
{
    public class AppMain : Form
    {
        [STAThread]
        static void Main()
        {
            GlobalKeyHook.SetHook();
            Application.Run(new AppMain());
        }

        private NotifyIcon systemTrayIcon;
        private System.ComponentModel.IContainer components;
        private ContextMenu systemTrayMenu;

        private ScreenShotPictureBox screenshotOverlay;

        private Rectangle screenBounds = Rectangle.Empty;
        private Point leftMostScreenCoordinate = new Point(0,0);

        public AppMain()
        {
            // set up global screenshot shortcut callback
            GlobalKeyHook.KeyPress += GlobalKeyHook_KeyPress;

            // figure out the size of all potential monitors combined
            CalculateScreenSize();

            InitializeComponent();
        }
        private void CalculateScreenSize()
        {

            foreach (Screen s in Screen.AllScreens)
            {
                screenBounds = Rectangle.Union(screenBounds, s.Bounds);

                if (s.WorkingArea.X < leftMostScreenCoordinate.X)
                {
                    leftMostScreenCoordinate.X = s.WorkingArea.X;
                    leftMostScreenCoordinate.Y = s.WorkingArea.Y;
                }
            }
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();            

            this.SuspendLayout();

            this.systemTrayMenu = new ContextMenu();
            this.systemTrayIcon = new NotifyIcon(this.components);

            // set up system tray icon
            this.systemTrayIcon.ContextMenu = this.systemTrayMenu;
            this.systemTrayIcon.Text = "QwikShot";
            this.systemTrayIcon.Visible = true;
            this.systemTrayIcon.Icon = new Icon(global::QwikShot.WinApp.Properties.Resources.icon_application, 32, 32);

            // set up form styles
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.Icon = new Icon(global::QwikShot.WinApp.Properties.Resources.icon_application, 32, 32);

            // set up the overlay for holding and resizing the screenshot
            screenshotOverlay = new ScreenShotPictureBox();

            // add overlay to the form
            this.Controls.Add(screenshotOverlay);

            this.ResumeLayout(false);
        }

        void GlobalKeyHook_KeyPress(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
