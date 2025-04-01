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
    public async Task<string> Convertion(string input, ILambdaContext context)
    {
        var playwright = await Playwright.CreateAsync();
        Console.WriteLine($"Hello Playwright!: {playwright}");
        return input.ToUpper();
    }
}
