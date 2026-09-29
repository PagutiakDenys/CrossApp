using Core.Dto;
using Core.Import;

string path = args.Length > 0
    ? args[0]
    : Path.Combine("data", "sample.csv");

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

int total = result.Items.Count + result.Errors.Count;
int accepted = result.Items.Count;
int skipped = result.Errors.Count;

double errorPercent = total == 0
    ? 0
    : skipped * 100.0 / total;

Console.WriteLine($"Файл: {path}");
Console.WriteLine($"Формат: {extension}");
Console.WriteLine();

Console.WriteLine($"Завантажено записів: {accepted}");

foreach (ProductDto product in result.Items.Take(5))
{
    Console.WriteLine(
        $" {product.Id,-6} " +
        $"{product.Sku,-10} " +
        $"{product.Name,-30} " +
        $"{product.Quantity,5} " +
        $"{product.Unit}");
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

return 0;