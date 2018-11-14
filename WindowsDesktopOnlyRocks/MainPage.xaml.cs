using System;
using System.Threading.Tasks;
using WindorLib;
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

            Initialize();
        }

        private async void Initialize()
        {
            await LoadInputFromClipboard();
            OnSourceUrlTextChanged(null, null);

            Clipboard.ContentChanged += OnClipboardChanged;
        }

        private async Task LoadInputFromClipboard()
        {
            const int MaxAttemptedSeconds = 2;
            var start = DateTime.UtcNow;
            for (; DateTime.UtcNow - start < TimeSpan.FromSeconds(MaxAttemptedSeconds); )
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

        private async void OnClipboardChanged(object sender, object e)
        {
            await LoadInputFromClipboard();
        }

        private void OnSourceUrlTextChanged(object sender, TextChangedEventArgs e)
        {
            var res = MobUrls.Convert(MobUrl.Text, out var targetUrl, out var remarks);
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
