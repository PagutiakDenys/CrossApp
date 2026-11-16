using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Services;

try
{
    ICatalogStore store = StoreFactory.Create(args);
    var service = new CatalogService(store);

    // Крок 3: підписник №1 – локальний метод (його можна відписати)
    var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
    Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
    File.WriteAllText(logPath, string.Empty);   // чистий лог на кожен запуск

    var order = new List<string>();             // щоб показати порядок викликів

    void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
    {
        var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
        Console.WriteLine(line);
        File.AppendAllText(logPath, line + Environment.NewLine);
        order.Add("лог");
    }

    service.Changed += OnCatalogChanged;

    // Крок 4: підписник №2 – лямбда із замиканням над changes
    int changes = 0;
    service.Changed += (_, _) => { changes++; order.Add("лічильник"); };

    // Крок 5: сценарій
    var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
    service.Receive(created.Id, 25);
    Console.WriteLine($"Подій отримано: {changes}");                       // 2
    Console.WriteLine($"Порядок викликів: {string.Join(" → ", order)}");   // лог → лічильник → лог → лічильник

    // Крок 6: відмови (підписка ще активна, але подій бути не повинно)
    Console.WriteLine("\nСценарій відмови:");
    foreach (var (id, qty) in new[] { (created.Id, -5), ("UNKNOWN-ID", 10) })
    {
        try
        {
            service.Receive(id, qty);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }
    Console.WriteLine($"Подій після відмов: {changes}\n");                  // усе ще 2

    // Відписка і «тиха» операція
    service.Changed -= OnCatalogChanged;
    service.Receive(created.Id, 5);
    Console.WriteLine($"Подій отримано: {changes}");                       // 3 (лише лічильник)

    // Крок 7: делегат, що повертає значення
    service.Add("SKU-102", "Роз'єм RJ45", "шт", 8);
    Console.WriteLine("\nТовари із залишком < 10:");
    foreach (var p in service.FindAll(p => p.Quantity < 10))
        Console.WriteLine($" {p.Name} — {p.Quantity} {p.Unit}");
}
catch (Exception ex)
{
    Console.WriteLine($"Критична помилка: {ex.Message}");
}