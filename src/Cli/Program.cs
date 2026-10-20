
using Core.Dto;
using Core.Import;
using Core.Domain;

string path = args.Length > 0
    ? args[0]
    : Path.Combine("data", "sample.csv");

Console.WriteLine("=== Лабораторна робота №4 ===");
Console.WriteLine("Модель предметної області Product");
Console.WriteLine();

// Демонстрація сутності Product
Console.WriteLine("=== Сценарій 1: успішні операції ===");

Product product = Product.Create(
    "P-001",
    "sku-001",
    "Цемент М400 25кг",
    "шт",
    100);

Console.WriteLine($"Створено: {product}");

product.RegisterArrival(50);
Console.WriteLine("Додано 50 одиниць.");

product.Issue(30);
Console.WriteLine("Видано 30 одиниць.");


Console.WriteLine($"Результат: {product}");

// Демонстрація перетворення Product у DTO та відновлення
ProductDto dto = product.ToDto();

Console.WriteLine(
    $"DTO: {dto.Id}, {dto.Sku}, {dto.Quantity} {dto.Unit}");

Product restored = Product.FromDto(dto);

Console.WriteLine($"Відновлений товар: {restored}");
Console.WriteLine();


// Демонстрація порушення інваріантів
Console.WriteLine("=== Сценарій 2: перевірка інваріантів ===");

TryDo(
    "Видача більшої кількості, ніж є на складі",
    () => product.Issue(1000));

TryDo(
    "Порожній SKU",
    () => Product.Create("P-002", " ", "Пісок", "т", 10));

TryDo(
    "Від'ємний початковий залишок",
    () => Product.Create("P-003", "SKU-003", "Цегла", "шт", -5));

TryDo(
    "Нульова кількість приходу",
    () => product.RegisterArrival(0));

Console.WriteLine();

// Імпорт даних із CSV або JSON
Console.WriteLine("=== Імпорт товарів із файлу ===");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

string extension = Path.GetExtension(path).ToLowerInvariant();

ImportResult<ProductDto> result = extension switch
{
    ".csv" => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),

    _ => new ImportResult<ProductDto>(
        [],
        [$"Непідтримуваний формат файлу: {extension}"])
};

int total = result.Items.Count + result.Errors.Count;
int accepted = result.Items.Count;
int skipped = result.Errors.Count;

double errorPercent = total == 0
    ? 0
    : skipped * 100.0 / total;

Console.WriteLine($"Файл: {path}");
Console.WriteLine($"Формат: {extension}");
Console.WriteLine($"Завантажено записів: {accepted}");

foreach (ProductDto item in result.Items.Take(5))
{
    Console.WriteLine(
        $" {item.Id,-6} " +
        $"{item.Sku,-10} " +
        $"{item.Name,-30} " +
        $"{item.Quantity,5} " +
        $"{item.Unit}");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine($"Пропущено рядків: {skipped}");

    foreach (string error in result.Errors)
    {
        Console.WriteLine($" ! {error}");
    }
}

Console.WriteLine(
    $"Усього: {total} | " +
    $"Прийнято: {accepted} | " +
    $"Пропущено: {skipped} | " +
    $"Помилки: {errorPercent:F1}%");

return 0;

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"{title}: помилка не виникла!");
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"{title}: {ex.GetType().Name} — {ex.Message}");
    }
}
