
namespace Core.Domain.Orders;

public enum OrderStatus
{
    Draft,
    Confirmed,
    Cancelled
}

public sealed class Order
{
    public string Id { get; }
    public OrderStatus Status { get; private set; }

    private Order(string id)
    {
        Id = id;
        Status = OrderStatus.Draft;
    }

    public static Order Create(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException(
                "Номер замовлення обов'язковий", nameof(id));

        return new Order(id.Trim());
    }

    public void Confirm()
    {
        ChangeStatus(OrderStatus.Confirmed);
    }

    public void Cancel()
    {
        ChangeStatus(OrderStatus.Cancelled);
    }

    private void ChangeStatus(OrderStatus newStatus)
    {
        bool allowed = (Status, newStatus) switch
        {
            (OrderStatus.Draft, OrderStatus.Confirmed) => true,
            (OrderStatus.Draft, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            _ => false
        };

        if (!allowed)
            throw new InvalidOperationException(
                $"Недопустимий перехід замовлення: {Status} -> {newStatus}");

        Status = newStatus;
    }
}
