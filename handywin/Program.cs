using System;
using System.Diagnostics;
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

        [STAThread]
        static void Main(string[] args)
        {
            if (Clipboard.ContainsText())
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
                    for (var attempt = 0; attempt < 15; attempt++)
                    {
                        var sim = new InputSimulator();
                        sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_S);
                        const int nChars = 256;
                        var p = GetForegroundWindow();
                        var sb = new StringBuilder(nChars);
                        if (GetWindowText(p, sb, nChars) > 0 && sb.ToString() == "Cortana")
                        {
                            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                            break;
                        }
                        Thread.Sleep(200);
                    }
                }
            }
        }
    }
}
