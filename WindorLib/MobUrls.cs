using System.Text.RegularExpressions;

namespace WindorLib
{
    public class MobUrls
    {
        private static bool IsMobOf(string url, string shortName)
        {
            var regex = new Regex($@"https*://[^/]*{shortName}\..*|[^/]*{shortName}\..*");
            return regex.Match(url).Success;
        }

        public static bool Convert(string mob, out string target, out string remarks)
        {
            var succeeded = false;
            target = "";
            remarks = "";
            if (IsMobOf(mob, "taobao") || IsMobOf(mob, "tmall"))
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
            else if (IsMobOf(mob, "youku"))
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
            else if (IsMobOf(mob, "facebook"))
            {
                remarks = "Successfully converted to desktop url from facebook mob url.\n"
                    + "尝试将脸书移设链接转换为桌面链接完成。";
                target = mob.Replace("m.facebook", "www.facebook");
                succeeded = true;
            }
            // TODO a generic treatment ?...
            else if (string.IsNullOrWhiteSpace(mob))
            {
                remarks = "Please provide mob url (link copied to clipboard will be captured).\n"
                    + "请提供移设链接（复制到剪贴板的链接将会被运用）。";
            }
            return succeeded;
        }
    }
}
