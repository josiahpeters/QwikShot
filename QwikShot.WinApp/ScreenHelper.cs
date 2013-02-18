using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace QwikShot.WinApp
{
    public class ScreenHelper
    {
        public static Bitmap CaptureScreenRegion(Rectangle regionBounds)
        {
            Bitmap screenshotImage = new Bitmap(regionBounds.Width, regionBounds.Height, PixelFormat.Format32bppArgb);

            Graphics screenShotGraphics = Graphics.FromImage(screenshotImage);

            screenShotGraphics.CopyFromScreen(regionBounds.X, regionBounds.Y, 0, 0, regionBounds.Size, CopyPixelOperation.SourceCopy);

            screenShotGraphics.Dispose();

            return screenshotImage;
        }

        public static Rectangle GetTotalScreenSize()
        {
            Rectangle screenPositionBounds = Rectangle.Empty;
            foreach (Screen s in Screen.AllScreens)
            {
                screenPositionBounds = Rectangle.Union(screenPositionBounds, s.Bounds);

                if (s.WorkingArea.X < screenPositionBounds.X)
                {
                    screenPositionBounds.X = s.WorkingArea.X;
                    screenPositionBounds.Y = s.WorkingArea.Y;
                }
            }
            return screenPositionBounds;
        }
        

        public static Rectangle GetActiveWindowBounds()
        {
            var windowHandle = GetForegroundWindow();

            RECT windowRect = new RECT();
            GetWindowRect(windowHandle, ref windowRect);

            Rectangle bounds = new Rectangle(windowRect.left, windowRect.top, windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);

            return bounds;
        }

        public static void ActivateApplication(string briefAppName)
        {
            Process[] procList = Process.GetProcessesByName(briefAppName);

            if (procList.Length > 0)
            {
                ShowWindow(procList[0].MainWindowHandle, SW_RESTORE);
                SetForegroundWindow(procList[0].MainWindowHandle);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        // Sets the window to be foreground
        [DllImport("User32")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        // Activate or minimize a window
        [DllImportAttribute("User32.DLL")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        
    }
}
