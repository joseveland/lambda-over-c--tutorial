using Amazon.Lambda.Core;
using Microsoft.Playwright;
using System.Diagnostics;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AsyncHandler;

public class IamTheClass
{
    
    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> Conversion(string input, ILambdaContext context)
    {
        Console.WriteLine($"Current Linux user: {GetCurrentLinuxUser()}");
        
        // Pointing to the installation path of the playwright browser into this lambda's container
        Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", "/tmp/ms-playwright/");
        Environment.SetEnvironmentVariable("FONTCONFIG_PATH", "/tmp/fonts");
        Environment.SetEnvironmentVariable("FONTCONFIG_FILE", "/tmp/fonts/fonts.conf");
        Environment.SetEnvironmentVariable("DBUS_SESSION_BUS_ADDRESS", "/dev/null");
        // Environment.SetEnvironmentVariable("XDG_RUNTIME_DIR", "/tmp/runtime-root");
        
        var playwright = await Playwright.CreateAsync();
        Console.WriteLine($"Playwright: {playwright}");
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {   // Headless means no browser window (UI) is needed. https://developer.chrome.com/docs/chromium/headless
            Headless = true,
            Args =
            [
                "--disable-gpu",
                "--disable-setuid-sandbox",
                "--no-sandbox"
            ]
        });
        Console.WriteLine($"Browser: {browser}");
        var page = await browser.NewPageAsync();
        Console.WriteLine($"Page: {page}");
        await page.GotoAsync("https://www.google.com");
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        var title = await page.TitleAsync();
        Console.WriteLine($"Title: {title}");
        return title;
    }
    
    public string GetCurrentLinuxUser()
    {
        try
        {
            // Method 1: Using 'whoami' command
            var whoami = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/whoami",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            whoami.Start();
            string user = whoami.StandardOutput.ReadToEnd().Trim();
            whoami.WaitForExit();
        
            if (!string.IsNullOrEmpty(user))
                return user;

            // Method 2: Using environment variables as fallback
            return Environment.GetEnvironmentVariable("USER") ?? 
                   Environment.GetEnvironmentVariable("USERNAME") ??
                   "unknown";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user: {ex.Message}");
            return "error";
        }
    }
}
