using Xunit;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;

namespace G3Operaciones.Tests.Domain.Orders;

public class OrderTests
{
    [Fact]
    public void NewOrder_ShouldStartAsPending()
    {
        var order = new Order(Guid.NewGuid(), null, null);

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(0m, order.TotalAmount);
    }

    [Fact]
    public void AddItem_ShouldUpdateOrderTotal()
    {
        var order = new Order(Guid.NewGuid(), null, null);

        order.AddItem(Guid.NewGuid(), quantity: 3, unitPrice: 12.50m);

        Assert.Equal(37.50m, order.TotalAmount);
        Assert.Single(order.Items);
    }

    [Fact]
    public void FinalizeOrder_WithoutItems_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid(), null, null);

        Assert.Throws<InvalidOperationException>(() => order.FinalizeOrder("ticket.pdf"));
    }

    [Fact]
    public void DeliveredOrder_CannotBeCancelled()
    {
        var order = new Order(Guid.NewGuid(), null, null);
        order.AddItem(Guid.NewGuid(), quantity: 1, unitPrice: 10m);
        order.FinalizeOrder("ticket.pdf");
        order.MarkAsDelivered();

        Assert.Throws<InvalidOperationException>(() => order.CancelOrder());
    }
}
