using Core.Abstractions;
using Core.Domain;
using Core.Services;
using Core.Storage;

try
{
    // 1. Створюємо сховище через фабрику
    ICatalogStore baseStore = StoreFactory.Create(args);

    // 2. Додаємо декоратор кешування
    ICatalogStore store = new CachingCatalogStore(baseStore);

    // 3. Створюємо сервіс
    var service = new CatalogService(store);

    Console.WriteLine($"Сховище: {baseStore.GetType().Name}");
    Console.WriteLine("Кешування: увімкнено");

    // 4. Додаємо товар
    var created = service.Add(
        "SKU-101",
        "Кабель UTP cat6",
        "м",
        50
    );

    Console.WriteLine(
        $"\nДодано товар: {created.Name}, ID: {created.Id}"
    );

    // 5. Реєструємо надходження
    service.Receive(created.Id, 25);
    Console.WriteLine("Надходження: +25");

    // 6. Пошук товарів за кількістю
    Console.WriteLine("\nТовари з кількістю більше 100:");

    var found = service.Search(p => p.Quantity > 100);

    foreach (var p in found)
    {
        Console.WriteLine($"{p.Name} — {p.Quantity} {p.Unit}");
    }

    // 7. Виводимо всі товари
    Console.WriteLine("\nСписок товарів:");

    foreach (var p in service.All())
    {
        Console.WriteLine(
            $" {p.Id} {p.Sku,-10} {p.Name,-22} {p.Quantity,5} {p.Unit}"
        );
    }

    // 8. Перевірка відсутнього товару
    Console.WriteLine("\nПеревірка відсутнього товару:");

    try
    {
        service.Receive("UNKNOWN-ID", 10);
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }

    // 9. Перевірка дублювання ID
    Console.WriteLine("\nПеревірка дублювання ID:");

    try
    {
        var duplicate = Product.Create(
            created.Id,
            "SKU-999",
            "Тестовий товар",
            "шт",
            10
        );

        store.Add(duplicate);
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }

    Console.WriteLine("\nРоботу завершено.");
}
catch (Exception ex)
{
    Console.WriteLine($"Критична помилка: {ex.Message}");
}