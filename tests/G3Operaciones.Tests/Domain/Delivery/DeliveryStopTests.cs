using Xunit;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;

namespace G3Operaciones.Tests.Domain.Delivery;

public class DeliveryStopTests
{
    [Fact]
    public void NewStop_ShouldStartAsPending()
    {
        var stop = CreateStop();

        Assert.Equal(DeliveryStopStatus.Pending, stop.Status);
    }

    [Fact]
    public void MarkAsDelivered_ShouldSetCompletionTimestamp()
    {
        var stop = CreateStop();

        stop.MarkAsDelivered();

        Assert.Equal(DeliveryStopStatus.Delivered, stop.Status);
        Assert.NotNull(stop.CompletedAt);
    }

    [Fact]
    public void MarkAsFailed_ShouldPreserveFailureReason()
    {
        var stop = CreateStop();

        stop.MarkAsFailed("Cliente ausente");

        Assert.Equal(DeliveryStopStatus.Failed, stop.Status);
        Assert.Equal("Cliente ausente", stop.FailureReason);
        Assert.NotNull(stop.CompletedAt);
    }

    private static DeliveryStop CreateStop() =>
        new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 19.4m, -99.1m, 1, 1);
}
