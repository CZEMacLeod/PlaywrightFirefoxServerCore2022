using Microsoft.Playwright;

namespace PlaywrightWindowsServerCore2022;

class Program
{
    public static async Task Main()
    {
        InstallFonts();
        InstallPowershell();
        InstallPlaywright();

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://playwright.dev/dotnet");
        await page.ScreenshotAsync(new() { Path = "screenshot.png" });
        var fileInfo = new System.IO.FileInfo("screenshot.png");
        Console.WriteLine("Image saved {0} {1}", fileInfo.FullName, fileInfo.Length);
    }

    private static void ExecuteCommandAndWait(string command, string? args = null)
    {
        Console.WriteLine("Starting: {0} {1}", command, args);
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = command;
        p.StartInfo.Arguments = args ?? string.Empty;
        p.StartInfo.CreateNoWindow = false;
        p.StartInfo.UseShellExecute = false;
        p.Start();
        p.WaitForExit();
        Console.WriteLine("Returned: {0}", p.ExitCode);
    }

    private static void InstallFonts()
    {
        foreach (var folder in System.IO.Directory.GetDirectories("C:\\WindowsSource\\"))
        {
            Console.WriteLine("WinSxS folder: {0}", folder);
        }
        ExecuteCommandAndWait("cmd.exe", "/c \"installfonts.cmd\"");
    }

    private static void InstallPlaywright() => ExecuteCommandAndWait("pwsh.exe", "playwright.ps1 install --with-deps chromium");

    private static void InstallPowershell() => ExecuteCommandAndWait("dotnet", "tool install --global PowerShell");
}