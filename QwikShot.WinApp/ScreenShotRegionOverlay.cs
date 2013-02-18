using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QwikShot.WinApp
{
    public class ScreenShotRegionOverlay : PictureBox
    {
        private ToolStrip toolStrip;

        private ToolStripButton buttonSave;
        private ToolStripButton buttonCopy;
        private ToolStripButton buttonNetwork;
        private ToolStripButton buttonInternet;
        private ToolStripButton buttonClose;

        private Bitmap fadedDesktopCapture;
        private Bitmap desktopCapture;

        private AppMain appMain;

        private bool dragging = false;
        private Point startPoint;
        private Point endPoint;
        private Rectangle captureRegion = Rectangle.Empty;
        // style for region drawing border
        private Pen regionBorderPen;

        public ScreenShotRegionOverlay(AppMain appMain)
        {
            this.appMain = appMain;
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            #region Creating and styling form controls

            this.SuspendLayout();

            toolStrip = new ToolStrip();
            toolStrip.Dock = DockStyle.None;
            toolStrip.Cursor = Cursors.Default;
            toolStrip.ImageScalingSize = new Size(32, 32);
            toolStrip.BackColor = Color.Transparent;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Renderer = new CustomToolStripRenderer();
            toolStrip.SuspendLayout();
            toolStrip.Visible = false;

            // buttons
            buttonSave = new ToolStripButton();
            buttonSave.Image = global::QwikShot.WinApp.Properties.Resources.image_save;
            buttonSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonSave.ImageAlign = ContentAlignment.MiddleCenter;
            buttonSave.ToolTipText = "Save Image and Copy to Clipboard";
            buttonSave.Margin = new Padding(0, 0, 4, 0);
            buttonSave.Click += buttonSave_Click;

            buttonCopy = new ToolStripButton();
            buttonCopy.Image = global::QwikShot.WinApp.Properties.Resources.image_copy;
            buttonCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonCopy.ImageAlign = ContentAlignment.MiddleCenter;
            buttonCopy.ToolTipText = "Copy Image to Clipboard";
            buttonCopy.Margin = new Padding(0, 0, 4, 0);
            buttonCopy.Click += buttonCopy_Click;

            buttonNetwork = new ToolStripButton();
            buttonNetwork.Image = global::QwikShot.WinApp.Properties.Resources.image_network;
            buttonNetwork.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonNetwork.ImageAlign = ContentAlignment.MiddleCenter;
            buttonNetwork.ToolTipText = "Share Over Network";
            buttonNetwork.Margin = new Padding(0, 0, 4, 0);
            buttonNetwork.Click += buttonNetwork_Click;

            buttonInternet = new ToolStripButton();
            buttonInternet.Image = global::QwikShot.WinApp.Properties.Resources.image_internet;
            buttonInternet.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonInternet.ImageAlign = ContentAlignment.MiddleCenter;
            buttonInternet.ToolTipText = "Share Through Internet";
            buttonInternet.Margin = new Padding(0, 0, 4, 0);
            buttonInternet.Click += buttonInternet_Click;

            buttonClose = new ToolStripButton();
            buttonClose.Image = global::QwikShot.WinApp.Properties.Resources.image_close;
            buttonClose.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buttonClose.ImageAlign = ContentAlignment.MiddleCenter;
            buttonClose.ToolTipText = "Cancel";
            buttonClose.Margin = new Padding(0, 0, 4, 0);
            buttonClose.Click += buttonClose_Click;

            // separators
            ToolStripSeparator separator1 = new ToolStripSeparator();
            separator1.AutoSize = false;
            separator1.Size = new Size(24, 0);

            ToolStripSeparator separator2 = new ToolStripSeparator();
            separator2.AutoSize = false;
            separator2.Size = new Size(48, 0);

            toolStrip.Items.AddRange(new ToolStripItem[] { buttonSave, buttonCopy, separator1, buttonNetwork, buttonInternet, separator2, buttonClose });

            toolStrip.ResumeLayout(false);

            this.Controls.Add(toolStrip); 

            #endregion

            regionBorderPen = new Pen(Color.FromArgb(100, 149, 237), 1);
            regionBorderPen.DashStyle = DashStyle.Solid;

            MouseMove += ScreenShotRegionOverlay_MouseMove;
            MouseDown += ScreenShotRegionOverlay_MouseDown;
            MouseUp += ScreenShotRegionOverlay_MouseUp;
        }

        private void DrawCaptureRegion()
        {
            CalculateCaptureRegion();

            this.Refresh();

            using (var g = this.CreateGraphics())
            {
                g.DrawImage(desktopCapture, captureRegion, captureRegion, GraphicsUnit.Pixel);
                g.DrawRectangle(regionBorderPen, captureRegion);
            }
        }

        private void CalculateCaptureRegion()
        {
            int x = 0;
            int y = 0;

            int width = 0;
            int height = 0;

            // calculate our drawing box size regardless of the direction you are dragging
            if (startPoint.X > endPoint.X)
            {
                x = endPoint.X;
                width = startPoint.X - endPoint.X;
            }
            else
            {
                x = startPoint.X;
                width = endPoint.X - startPoint.X;
            }

            if (startPoint.Y > endPoint.Y)
            {
                y = endPoint.Y;
                height = startPoint.Y - endPoint.Y;
            }
            else
            {
                y = startPoint.Y;
                height = endPoint.Y - startPoint.Y;
            }

            captureRegion = new Rectangle(x, y, width, height);
        }

        #region Mouse Events for region drawing

        void ScreenShotRegionOverlay_MouseUp(object sender, MouseEventArgs e)
        {
            // end point
            endPoint = e.Location;
            // stop dragging
            dragging = false;

            DrawCaptureRegion();
        }

        void ScreenShotRegionOverlay_MouseDown(object sender, MouseEventArgs e)
        {
            // start point
            startPoint = e.Location;
            // start dragging
            dragging = true;
        }

        void ScreenShotRegionOverlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                // draw draggable area
                endPoint = e.Location;
                DrawCaptureRegion();
            }

        }
        
        #endregion

        #region toolbar button even handlers

        void buttonClose_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void buttonInternet_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void buttonNetwork_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void buttonCopy_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void buttonSave_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        internal void ResizeToBounds(Rectangle screenBounds)
        {
            this.Width = screenBounds.Width;
            this.Height = screenBounds.Height;
            this.Left = 0;
            this.Top = 0;
        }

        #endregion

        internal void CaptureRegion(Bitmap desktopCapture)
        {
            this.desktopCapture = desktopCapture;
            fadedDesktopCapture = new Bitmap(desktopCapture, desktopCapture.Width, desktopCapture.Height);

            using (Graphics gfx = Graphics.FromImage(fadedDesktopCapture))
            {
                gfx.FillRectangle(new SolidBrush(Color.FromArgb(128, 192, 192, 192)), 0, 0, fadedDesktopCapture.Width, fadedDesktopCapture.Height);
            }

            this.Image = fadedDesktopCapture;


            appMain.MakeActive();
        }
    }
}
