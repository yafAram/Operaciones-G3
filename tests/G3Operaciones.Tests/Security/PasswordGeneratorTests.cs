using Xunit;
using API.G3Operaciones.Garcias.MX.Infrastructure.Security;

namespace G3Operaciones.Tests.Security;

public class PasswordGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnRequestedLength()
    {
        var generator = new PasswordGenerator();

        var password = generator.Generate(20);

        Assert.Equal(20, password.Length);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(257)]
    public void Generate_ShouldRejectUnsafeLength(int length)
    {
        var generator = new PasswordGenerator();

        Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(length));
    }
}
