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
            //Application.Run(new Settings());
        }

        private NotifyIcon systemTrayIcon;
        private System.ComponentModel.IContainer components;
        private ContextMenu systemTrayMenu;

        private ScreenShotRegionOverlay screenshotOverlay;

        private Rectangle screenPositionBounds = Rectangle.Empty;

        // Image references
        private Bitmap desktopCapture;

        public AppMain()
        {
            // set up global screenshot shortcut callback
            GlobalKeyHook.KeyPress += GlobalKeyHook_KeyPress;

            // figure out the size of all potential monitors combined
            InitializeComponent();
        }
        private void CalculateAndResizeToScreenSize()
        {
            // calculate the total desktop size for all monitors
            screenPositionBounds = ScreenHelper.GetTotalScreenSize();

            // resize form to fit max screen size
            this.ResizeToBounds(screenPositionBounds);
            // resize overlay to fit max screen size
            screenshotOverlay.ResizeToBounds(screenPositionBounds);
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.KeyDown += AppMain_KeyDown;

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

            // allow the form to start on a monitor that isn't the main screen
            this.StartPosition = FormStartPosition.Manual;

            // set up the overlay for holding and resizing the screenshot
            screenshotOverlay = new ScreenShotRegionOverlay(this);

            // add overlay to the form
            this.Controls.Add(screenshotOverlay);

            // resize form and resize overlay to take up entire desktop
            CalculateAndResizeToScreenSize();

            this.ResumeLayout(false);
        }

        private void ResizeToBounds(Rectangle screenBounds)
        {
            this.Width = screenBounds.Width;
            this.Height = screenBounds.Height;
            this.Left = screenBounds.X;
            this.Top = screenBounds.Y;
        }

        private void CaptureScreenshot(Rectangle captureBounds)
        {
            // capture entire display
            desktopCapture = ScreenHelper.CaptureScreenRegion(screenPositionBounds);

            var regionCapture = true;

            // show overlay
            if (regionCapture)
            {
                screenshotOverlay.CaptureRegion(desktopCapture);
            }
            // save just the specified bounds
            else
                SaveBounds(captureBounds);
            // do something with it
        }

        private void SaveBounds(Rectangle captureBounds)
        {

        }

        internal void MakeActive()
        {
            this.Visible = true;
            this.Focus();
        }

        private void CancelCapture()
        {
            this.Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Hide form window
            this.Visible = false;
            // Remove from taskbar
            this.ShowInTaskbar = false;

            System.Threading.Thread.Sleep(1000);

            ScreenHelper.GetActiveWindowBounds();

            base.OnLoad(e);
        }

        private void AppMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                CancelCapture();
        }

        void GlobalKeyHook_KeyPress(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            CaptureScreenshot(screenPositionBounds);
        }
    }
}
