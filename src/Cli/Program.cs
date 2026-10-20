using Core.Dto;
using Core.Import;
using Core.Domain;
using Core.Domain.Library;
using Core.Domain.Orders;

string path = args.Length > 0
    ? args[0]
    : Path.Combine("data", "sample.csv");

Console.WriteLine("=== Лабораторна робота №4 ===");
Console.WriteLine("Модель предметної області Product");
Console.WriteLine();

// Сценарій 1: успішні операції
Console.WriteLine("=== Сценарій 1: успішні операції ===");

Product product = Product.Create(
    "P-001",
    "sku-001",
    "Цемент М400 25кг",
    "шт",
    100);

Console.WriteLine($"Створено: {product}");

product.RegisterArrival(60);
Console.WriteLine("Додано 60 одиниць.");

product.Issue(30);
Console.WriteLine("Видано 30 одиниць.");

Console.WriteLine($"Результат: {product}");
Console.WriteLine();

// Перетворення Product у DTO та відновлення
Console.WriteLine("=== Перевірка DTO ===");

ProductDto dto = product.ToDto();

Console.WriteLine(
    $"DTO: {dto.Id}, {dto.Sku}, {dto.Quantity} {dto.Unit}");

Product restored = Product.FromDto(dto);

Console.WriteLine($"Відновлений товар: {restored}");
Console.WriteLine();

// Сценарій 2: порушення інваріантів
Console.WriteLine("=== Сценарій 2: перевірка інваріантів ===");

TryDo(
    "Видача більшої кількості, ніж є на складі",
    () => product.Issue(1000));

TryDo(
    "Порожній SKU",
    () => Product.Create(
        "P-002", " ", "Пісок", "т", 10));

TryDo(
    "Від'ємний початковий залишок",
    () => Product.Create(
        "P-003", "SKU-003", "Цегла", "шт", -5));

TryDo(
    "Нульова кількість приходу",
    () => product.RegisterArrival(0));

Console.WriteLine();

// Імпорт товарів із CSV або JSON
Console.WriteLine("=== Імпорт товарів із файлу ===");

if (!File.Exists(path))
{
    Console.WriteLine(
        $"Файл не знайдено: {Path.GetFullPath(path)}");

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

int accepted = result.Items.Count;
int skipped = result.Errors.Count;
int total = accepted + skipped;

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
    Console.WriteLine();
    Console.WriteLine($"Пропущено рядків: {skipped}");

    foreach (string error in result.Errors)
    {
        Console.WriteLine($" ! {error}");
    }
}

Console.WriteLine();
Console.WriteLine(
    $"Усього: {total} | " +
    $"Прийнято: {accepted} | " +
    $"Пропущено: {skipped} | " +
    $"Помилки: {errorPercent:F1}%");

// Додаткове завдання 1:
// Перетворення DTO на доменні сутності
Console.WriteLine();
Console.WriteLine("=== Додаткове завдання 1: ProductImportMapper ===");

ProductMappingResult mapped = ProductImportMapper.Map(result);

Console.WriteLine($"Створено сутностей: {mapped.Items.Count}");
Console.WriteLine($"Усього помилок імпорту та мапінгу: {mapped.Errors.Count}");

foreach (string error in mapped.Errors)
{
    Console.WriteLine($" ! {error}");
}

// Додаткове завдання 2:
// Обмеження кількості відкритих видач
Console.WriteLine();
Console.WriteLine("=== Додаткове завдання 2: LoanService ===");

Reader reader = Reader.Create("R-001");
var loans = new List<Loan>();

for (int i = 1; i <= 5; i++)
{
    BookCopy copy = BookCopy.Create($"B-{i:000}");
    Loan loan = LoanService.IssueCopy(reader, copy, loans);
    loans.Add(loan);
}

Console.WriteLine($"Відкритих видач читача: {loans.Count}");

TryDo(
    "Спроба оформити шосту видачу",
    () =>
    {
        BookCopy copy = BookCopy.Create("B-006");
        Loan loan = LoanService.IssueCopy(reader, copy, loans);
        loans.Add(loan);
    });

// Додаткове завдання 3:
// Перевірка переходів статусів замовлення
Console.WriteLine();
Console.WriteLine("=== Додаткове завдання 3: OrderStatus ===");

Order order = Order.Create("ORD-001");

Console.WriteLine($"Початковий статус: {order.Status}");

order.Confirm();
Console.WriteLine($"Після підтвердження: {order.Status}");

order.Cancel();
Console.WriteLine($"Після скасування: {order.Status}");

TryDo(
    "Повторне підтвердження скасованого замовлення",
    () => order.Confirm());

Console.WriteLine();
Console.WriteLine("=== Перевірку завершено ===");

return 0;

// Допоміжний метод для перевірки інваріантів
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