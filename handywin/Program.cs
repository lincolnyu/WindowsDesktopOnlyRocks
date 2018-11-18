using ScreenShotDemo;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using WindorLib;
using WindowsInput;
using WindowsInput.Native;

namespace handywin
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;


        static bool GetForegroundWindowAndTitle(out IntPtr handle, out string title)
        {
            const int nChars = 256;
            handle = GetForegroundWindow();
            var sb = new StringBuilder(nChars);
            if (GetWindowText(handle, sb, nChars) > 0)
            {
                title = sb.ToString();
                return true;
            }
            title = "";
            return false;
        }

        static bool WaitForForeground(string containedTitle, out IntPtr handle,
            out string actualTitle)
            => WaitForForeground(containedTitle, TimeSpan.FromSeconds(3), out handle, out actualTitle);
        
        static bool WaitForForeground(string title, TimeSpan timeout, out IntPtr handle, 
            out string actualTitle)
        {
            const int intervalMs = 200;
            var interval = TimeSpan.FromMilliseconds(intervalMs);
            var start = DateTime.UtcNow;
            while (true)
            {
                if (GetForegroundWindowAndTitle(out handle, out actualTitle) 
                    && actualTitle.ToLower().Contains(title.ToLower()))
                {
                    return true;
                }
                if (DateTime.UtcNow - start + interval <= timeout)
                {
                    Thread.Sleep(interval);
                }
                else
                {
                    handle = IntPtr.Zero;
                    actualTitle = "";
                    return false;
                }
            }
        }

        static bool WaitForVisualComponentOnScreen(Bitmap vc, out Point point)
            => WaitForVisualComponentOnScreen(vc, TimeSpan.FromSeconds(10), out point);

        static bool WaitForVisualComponentOnScreen(Bitmap vc, TimeSpan timeout, out Point point)
        {
            const int intervalMs = 200;
            var interval = TimeSpan.FromMilliseconds(intervalMs);
            var start = DateTime.UtcNow;
            // https://stackoverflow.com/questions/1163761/capture-screenshot-of-active-window
            var sc = new ScreenCapture();
            while (true)
            {
                if (sc.CaptureScreen() is Bitmap screenBmp)
                {
                    if (screenBmp.SearchAlmostExactMatch(vc, out point))
                    {
                        point = new Point(point.X + vc.Width / 2, point.Y + vc.Height / 2);
                        return true;
                    }
                    Thread.Sleep(interval);
                }
                else
                {
                    point = default(Point);
                    return false;
                }
            }
        }

        private static void MouseClick(Point point)
            => MouseClick(point.X, point.Y);

        private static void MouseClick(int xpos, int ypos)
        {
            //https://stackoverflow.com/questions/8272681/how-can-i-simulate-a-mouse-click-at-a-certain-position-on-the-screen
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        [STAThread]
        static void Main(string[] args)
        {
            if (Clipboard.ContainsImage())
            {
                Process.Start("microsoft-edge:http://images.bing.com");
                if (WaitForForeground("Bing Image", TimeSpan.FromSeconds(10),
                    out var handle, out var actualTitle))
                {
                    var cam = Properties.Resources.camera;
                    if (WaitForVisualComponentOnScreen(cam, out var point))
                    {
                        MouseClick(point);

                        var paste = Properties.Resources.pasteImageOrUrl;
                        if (WaitForVisualComponentOnScreen(paste, out point))
                        {
                            MouseClick(point);

                            var sim = new InputSimulator();
                            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        }
                    }
                }
            }
            else if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                if (MobUrls.Convert(text, out var target, out var remarks))
                {
                    Process.Start(target); // opens with default browser
                }
                else if (BingMaps.IsSortOfAddress(text))
                {
                    var url = BingMaps.CreateBingMapsUrl(text);
                    Process.Start($"bingmaps:{url}");
                }
                else
                {
                    // fallback
                    var sim = new InputSimulator();
                    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_S);
                    if (WaitForForeground("Cortana", out var dummy, out var dummy2))
                    {
                        sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                    }
                }
            }
        }
    }
}
