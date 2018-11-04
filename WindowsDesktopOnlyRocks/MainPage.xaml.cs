using System;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsDesktopOnlyRocks
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Clipboard.ContentChanged += OnClipboardChanged;
        }

        private async void OnClipboardChanged(object sender, object e)
        {
            for (var a = 0; a < 1000; a++)
            {
                try
                {
                    var dp = Clipboard.GetContent();
                    if (dp.Contains(StandardDataFormats.Text))
                    {
                        var txt = await dp.GetTextAsync();
                        if (txt != TargetUrl.Text)
                        {
                            MobUrl.Text = txt;
                        }
                    }
                    break;
                }
                catch (Exception)
                {
                }
            }
        }

        private void OnSourceUrlTextChanged(object sender, TextChangedEventArgs e)
        {
            var res = Convert(MobUrl.Text, out var targetUrl, out var remarks);
            if (res)
            {
                Remarks.Text = remarks;
            }
            else
            {
                Remarks.Text = string.IsNullOrWhiteSpace(remarks) ?
                    ("Unable to convert. Please provide supported URL (link copied to clipboard will be captured).\n"
                    + "转换失败。请提供被支持的链接（复制到剪贴板的链接将会被运用）。") 
                    : remarks;
            }
            TargetUrl.Text = targetUrl;
            CopyUrl.IsEnabled = res;
            OpenInBrowser.IsEnabled = res;
        }

        private bool Convert(string mob, out string target, out string remarks)
        {
            var succeeded = false;
            target = "";
            remarks = "";
            if (mob.Contains("taobao") || mob.Contains("tmall"))
            {
                var regex = new Regex(@"[?&]id=([0-9]+)");
                var match = regex.Match(mob);
                if (match.Success)
                {
                    remarks = "Successfully converted to tmall url from mobile taobao.\n"
                        + "尝试将淘宝移设链接转换为天猫链接完成。";
                    var group = match.Groups[1];
                    var id = group.Value;
                    target = $"https://detail.tmall.com/item.htm?id={id}";
                    succeeded = true;
                }
                else
                {
                    remarks = "Unrecognized taobao url.\n无法识别的淘宝链接。";
                }
            }
            else if (mob.Contains("youku"))
            {
                var regex = new Regex(@"id_([0-9A-Za-z=]+)");
                var match = regex.Match(mob);
                if (match.Success)
                {
                    remarks = "Successfully converted to desktop url from youku mob url.\n"
                        + "尝试将优酷移设链接转换为桌面链接完成。";
                    var group = match.Groups[1];
                    var id = group.Value;
                    target = $"https://v.youku.com/v_show/id_{id}.html";
                    succeeded = true;
                }
                else
                {
                    remarks = "Unrecognized youku url.\n无法识别的优酷链接。";
                }
            }
            else if (string.IsNullOrWhiteSpace(mob))
            {
                remarks = "Please provide mob url (link copied to clipboard will be captured).\n"
                    + "请提供移设链接（复制到剪贴板的链接将会被运用）。";
            }
            return succeeded;
        }

        private void OnCopyUrlClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dp = new DataPackage();
            dp.SetText(TargetUrl.Text);
            Clipboard.SetContent(dp);
            Clipboard.Flush();
        }

        private async void OnOpenInBrowserClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var uri = new Uri(TargetUrl.Text);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
