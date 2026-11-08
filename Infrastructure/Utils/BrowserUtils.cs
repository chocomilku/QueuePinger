using System.Diagnostics;

namespace Infrastructure.Utils;

public static class BrowserUtils
{
    public static bool OpenBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}