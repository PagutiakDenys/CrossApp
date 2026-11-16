using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class CatalogService
{
    private readonly ICatalogStore _store;
    private readonly INotificationSink? _sink;

    public event EventHandler<CatalogChangedEventArgs>? Changed;

    // sink необов'язковий: без нього сервіс працює лише з подією
    public CatalogService(ICatalogStore store, INotificationSink? sink = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _sink = sink;
    }

    private void OnChanged(string id, CatalogChangeKind kind, int delta, int quantityAfter)
    {
        var e = new CatalogChangedEventArgs(id, kind, delta, quantityAfter);

        _sink?.Notify(e);              // 1) явна залежність, один одержувач
        Changed?.Invoke(this, e);      // 2) подія, будь-яка кількість підписників
    }

    public Product Add(string sku, string name, string unit, int quantity)
    {
        var product = Product.Create(
            Guid.NewGuid().ToString("N")[..8], sku, name, unit, quantity);

        _store.Add(product);
        OnChanged(product.Id, CatalogChangeKind.Added, quantity, product.Quantity);
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");

        product.RegisterArrival(quantity);   // виняток тут → повідомлень не буде
        _store.Update(product);
        OnChanged(product.Id, CatalogChangeKind.Received, quantity, product.Quantity);
    }

    public void Issue(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");

        product.Issue(quantity);
        _store.Update(product);
        OnChanged(product.Id, CatalogChangeKind.Issued, -quantity, product.Quantity);
    }

    public bool Remove(string id)
    {
        var product = _store.GetById(id);
        if (product is null || !_store.Remove(id)) return false;

        OnChanged(id, CatalogChangeKind.Removed, -product.Quantity, 0);
        return true;
    }

    public IReadOnlyList<Product> All() => _store.List();

    public Product? Find(string id) => _store.GetById(id);

    public IReadOnlyList<Product> FindAll(Func<Product, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _store.List().Where(predicate).ToList();
    }
}