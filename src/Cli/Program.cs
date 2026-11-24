using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Import;
using Core.Reports;
using Core.Services;

try
{
    // ---------- Складання залежностей ----------
    ICatalogStore store = StoreFactory.Create(args);

    var logPath = Path.Combine(
        AppContext.BaseDirectory,
        "logs",
        "sink.log");

    Directory.CreateDirectory(
        Path.GetDirectoryName(logPath)!);

    File.WriteAllText(logPath, string.Empty);

    INotificationSink sink = new CompositeSink(
        new ConsoleSink(),
        new FileSink(logPath));

    var service = new CatalogService(store, sink);

    // ---------- Визначення команди ----------
    var command = args.FirstOrDefault(
        a => !a.StartsWith("--")) ?? "demo";

    // ---------- Підкоманди звітів ----------
    if (command.StartsWith("report-"))
    {
        var items = service.All();

        switch (command)
        {
            case "report-top":
            {
                Console.WriteLine("=== ТОП-5 товарів ===");

                Print(ReportService.TopByQuantity(items, 5));
                break;
            }

            case "report-low":
            {
                Console.WriteLine("=== Товари з малим залишком ===");

                Print(ReportService.LowStock(items, 10));
                break;
            }

            case "report-groups":
            {
                Console.WriteLine("=== Групування за одиницями ===");

                foreach (var g in ReportService.ByUnit(items))
                {
                    Console.WriteLine(
                        $" {g.Key,-6} позицій: {g.Count,3} " +
                        $"сумарно: {g.TotalQuantity,6}");
                }

                break;
            }

            case "report-sum":
            {
                Console.WriteLine("=== Загальна статистика ===");

                var t = ReportService.Totals(items);

                Console.WriteLine(
                    $" позицій {t.Items}, " +
                    $"всього {t.Total}, " +
                    $"середнє {t.Average:F2}, " +
                    $"макс {t.Max}");

                break;
            }

            case "report-deferred":
            {
                Console.WriteLine(
                    "=== Відкладене виконання LINQ ===");

                var result =
                    ReportService.DeferredExecution(items);

                Console.WriteLine(
                    $"Дефіцитних до змін: {result.Before}");

                Console.WriteLine(
                    $"Дефіцитних після змін: {result.After}");

                Console.WriteLine(
                    $"У знімку лишилось: {result.SnapshotCount}");

                break;
            }

            default:
            {
                PrintHelp();
                break;
            }
        }
    }
    else
    {
        // ---------- Для порівняння: подія з лічильником ----------
        int changes = 0;

        EventHandler<CatalogChangedEventArgs> counter =
            (_, _) => changes++;

        service.Changed += counter;

        // ---------- Завдання 1 ----------
        Console.WriteLine("=== Завдання 1: sink vs подія ===");

        var created = service.Add(
            "SKU-101",
            "Кабель UTP cat6",
            "м",
            50);

        service.Receive(created.Id, 25);

        Console.WriteLine(
            $"Подій отримано лічильником: {changes}");

        // Відмова: ні sink, ні подія не спрацьовують
        try
        {
            service.Receive(created.Id, -5);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        Console.WriteLine(
            $"Подій після відмови: {changes}");

        // Відписуємося від події
        service.Changed -= counter;

        service.Receive(created.Id, 5);

        Console.WriteLine(
            $"Подій отримано лічильником: {changes}");

        Console.WriteLine(
            $"Рядків у sink.log: " +
            $"{File.ReadAllLines(logPath).Length}");

        // ---------- Завдання 2 ----------
        Console.WriteLine(
            "\n=== Завдання 2: звіт про пропущені рядки ===");

        var csvPath = Path.Combine(
            AppContext.BaseDirectory,
            "data",
            "sample.csv");

        // Варіант А: у консоль
        var result = ProductCsvImporter.Load(
            csvPath,
            msg => Console.WriteLine($"Пропущено: {msg}"));

        Console.WriteLine(
            $"Прийнято: {result.Items.Count}, " +
            $"відхилено: {result.Errors.Count}");

        // Варіант Б: у список
        var skipped = new List<string>();

        ProductCsvImporter.Load(csvPath, skipped.Add);

        Console.WriteLine(
            $"Зібрано в список: {skipped.Count}");

        // Варіант В: у файл
        var skipLog = Path.Combine(
            AppContext.BaseDirectory,
            "logs",
            "skipped.log");

        ProductCsvImporter.Load(
            csvPath,
            msg => File.AppendAllText(
                skipLog,
                msg + Environment.NewLine));

        Console.WriteLine(
            $"Файл skipped.log створено: {File.Exists(skipLog)}");

        // Варіант Г: без звіту
        ProductCsvImporter.Load(csvPath);
    }
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Критична помилка: {ex.Message}");
}

// ---------- Виведення рядків звіту ----------
static void Print(IReadOnlyList<StockLine> lines)
{
    foreach (var l in lines)
    {
        Console.WriteLine(
            $" {l.Sku,-10} " +
            $"{l.Name,-22} " +
            $"{l.Quantity,6} " +
            $"{l.Unit}");
    }
}

// ---------- Довідка ----------
static void PrintHelp()
{
    Console.WriteLine("Доступні команди:");

    Console.WriteLine("report-top");
    Console.WriteLine("report-low");
    Console.WriteLine("report-groups");
    Console.WriteLine("report-sum");
    Console.WriteLine("report-deferred");
}