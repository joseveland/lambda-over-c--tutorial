using Amazon.Lambda.Core;
using Microsoft.Playwright;

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
        var playwright = await Playwright.CreateAsync();
        Console.WriteLine($"Playwright: {playwright}");
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {   // Headless means no browser window (UI) is needed. https://developer.chrome.com/docs/chromium/headless
            Headless = true
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
}
