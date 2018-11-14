using System;
using System.Diagnostics;
using System.Windows;
using WindorLib;

namespace handywin
{
    class Program
    {
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
            }
        }
    }
}
