using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

namespace AsyncHandler.Tests;

public class IamTheClassTest
{
    [Fact]
    public async Task TestToUpper()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new IamTheClass();
        var context = new TestLambdaContext();
        var casing = await function.Conversion("hello world", context);

        Assert.Equal("HELLO WORLD", casing);
    }
}
