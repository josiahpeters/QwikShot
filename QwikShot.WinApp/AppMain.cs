using QwikShot.WinApp.Sharing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private string imgurLink;

        // Image references
        private Bitmap desktopCapture;

        public AppMain()
        {
            // set up global screenshot shortcut callback
            GlobalKeyHook.KeyPress += GlobalKeyHook_KeyPress;

            // figure out the size of all potential monitors combined
            InitializeComponent();

            //imgur.AuthorizePin("");
            //imgur.GetAuthToken("e1c63da25f4eb996723b4709490c423abe193600");
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
            components = new System.ComponentModel.Container();

            KeyDown += AppMain_KeyDown;

            SuspendLayout();

            systemTrayMenu = new ContextMenu();
            systemTrayMenu.MenuItems.Add("Exit", OnExit);
            systemTrayIcon = new NotifyIcon(this.components);
            systemTrayIcon.BalloonTipClicked += systemTrayIcon_BalloonTipClicked;

            // set up system tray icon
            systemTrayIcon.ContextMenu = this.systemTrayMenu;
            systemTrayIcon.Text = "QwikShot";
            systemTrayIcon.Visible = true;
            systemTrayIcon.Icon = new Icon(global::QwikShot.WinApp.Properties.Resources.icon_application, 32, 32);

            // set up form styles
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Normal;
            Cursor = System.Windows.Forms.Cursors.Cross;
            Icon = new Icon(global::QwikShot.WinApp.Properties.Resources.icon_application, 32, 32);
            

            // set up the overlay for holding and resizing the screenshot
            screenshotOverlay = new ScreenShotRegionOverlay(this);
            screenshotOverlay.RegionCaptured += screenshotOverlay_RegionCaptured;

            // add overlay to the form
            Controls.Add(screenshotOverlay);

            // resize form and resize overlay to take up entire desktop
            CalculateAndResizeToScreenSize();

            ResumeLayout(false);
        }

        void systemTrayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(imgurLink))
            {
                ProcessStartInfo sInfo = new ProcessStartInfo(imgurLink);
                Process.Start(sInfo);
            }
        }

        void screenshotOverlay_RegionCaptured(object sender, RegionCapturedEventArgs e)
        {
            Imgur imgur = new Imgur();

            CancelCapture();

            var response = imgur.UploadImage(GetImageFromScreenshotByBounds(e.Bounds));

            //var deserialized = DynamicJson.Deserialize(response);
            //var deserialized = JsonObject.Parse(response);

            //imgurLink = deserialized.Object("data").Get("link");

            var search = "link\":\"";
            int position = response.IndexOf(search);

            response = response.Substring(position + search.Length);
            
            position = response.IndexOf("\"}");

            imgurLink = response.Substring(0, position).Replace("\\", "");            

            systemTrayIcon.ShowBalloonTip(1000, "Screenshot Uploaded", String.Format("Screenshot uploaded to imgur.\n Url: {0} copied to clipboard.", imgurLink), ToolTipIcon.Info);

            Clipboard.SetText(imgurLink);
        }

        private void ResizeToBounds(Rectangle screenBounds)
        {
            // allow the form to start on a monitor that isn't the main screen
            StartPosition = FormStartPosition.Manual;

            Width = screenBounds.Width;
            Height = screenBounds.Height;
            Left = screenBounds.X;
            Top = screenBounds.Y;
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
                GetImageFromScreenshotByBounds(captureBounds);
            // do something with it
        }

        private Bitmap GetImageFromScreenshotByBounds(Rectangle captureBounds)
        {
            var image = new Bitmap(captureBounds.Width, captureBounds.Height);

            using (Graphics gfx = Graphics.FromImage(image))
            {
                gfx.DrawImage(desktopCapture, 0,0, captureBounds, GraphicsUnit.Pixel);
            }

            return image;
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

        #region Form Event Handlers

        private void AppMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                CancelCapture();
            if (e.KeyCode == Keys.Enter)
                screenshotOverlay.FinishCapture();
        }

        void GlobalKeyHook_KeyPress(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            CaptureScreenshot(screenPositionBounds);
        }

        #endregion

        #region Event Override Methods

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



        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
            GlobalKeyHook.Unhook();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                systemTrayIcon.Dispose();
            }
            base.Dispose(isDisposing);
        } 

        #endregion
    }
}
