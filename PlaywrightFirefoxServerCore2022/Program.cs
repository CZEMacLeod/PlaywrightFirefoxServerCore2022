using Microsoft.Playwright;
using System.Diagnostics;

namespace PlaywrightFirefoxServerCore2022;

class Program
{
    public static async Task Main()
    {
        InstallPowershell();
        InstallPlaywright();

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Firefox.LaunchAsync(new() { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://playwright.dev/dotnet");
        await page.ScreenshotAsync(new() { Path = "screenshot.png" });
    }

    private static void InstallPlaywright()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "pwsh.exe";
        p.StartInfo.Arguments = "playwright.ps1 install firefox";
        p.Start();
        p.WaitForExit();
    }

    private static void InstallPowershell()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "dotnet";
        p.StartInfo.Arguments = "tool install --global PowerShell";
        p.Start();
        p.WaitForExit();
    }
}