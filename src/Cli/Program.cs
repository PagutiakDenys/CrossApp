using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Import;
using Core.Services;

try
{
    // ---------- Складання залежностей (composition root) ----------
    ICatalogStore store = StoreFactory.Create(args);

    var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "sink.log");
    Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
    File.WriteAllText(logPath, string.Empty);                 // чистий лог на запуск

    INotificationSink sink = new CompositeSink(
        new ConsoleSink(),
        new FileSink(logPath));

    var service = new CatalogService(store, sink);            // sink передано через конструктор

    // ---------- Для порівняння: подія з лічильником ----------
    int changes = 0;
    EventHandler<CatalogChangedEventArgs> counter = (_, _) => changes++;
    service.Changed += counter;

    // ---------- Сценарій ----------
    Console.WriteLine("=== Завдання 1: sink vs подія ===");

    var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
    service.Receive(created.Id, 25);
    Console.WriteLine($"Подій отримано лічильником: {changes}");            // 2

    // Відмова: ні sink, ні подія не спрацьовують
    try
    {
        service.Receive(created.Id, -5);
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }
    Console.WriteLine($"Подій після відмови: {changes}");                   // 2

    // Подію можна відписати, sink – ні (він зафіксований у конструкторі)
    service.Changed -= counter;
    service.Receive(created.Id, 5);

    Console.WriteLine($"Подій отримано лічильником: {changes}");            // 2 (відписались)
    Console.WriteLine($"Рядків у sink.log: {File.ReadAllLines(logPath).Length}"); // 3 (sink працює далі)

    // ---------- Завдання 2: Action<string> ----------
    Console.WriteLine("\n=== Завдання 2: звіт про пропущені рядки ===");

    var csvPath = Path.Combine(AppContext.BaseDirectory, "data", "sample.csv");

    // Варіант А: у консоль
    var result = ProductCsvImporter.Load(
        csvPath,
        msg => Console.WriteLine($"Пропущено: {msg}"));

    Console.WriteLine($"Прийнято: {result.Items.Count}, відхилено: {result.Errors.Count}");

    // Варіант Б: у список (той самий метод, інша поведінка)
    var skipped = new List<string>();
    ProductCsvImporter.Load(csvPath, skipped.Add);
    Console.WriteLine($"Зібрано в список: {skipped.Count}");

    // Варіант В: у файл
    var skipLog = Path.Combine(AppContext.BaseDirectory, "logs", "skipped.log");
    ProductCsvImporter.Load(csvPath, msg => File.AppendAllText(skipLog, msg + Environment.NewLine));
    Console.WriteLine($"Файл skipped.log створено: {File.Exists(skipLog)}");

    // Варіант Г: без звіту (параметр необов'язковий)
    ProductCsvImporter.Load(csvPath);
}
catch (Exception ex)
{
    Console.WriteLine($"Критична помилка: {ex.Message}");
}