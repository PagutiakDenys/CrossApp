
using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
    // 1. Товари з малим залишком
    public static IReadOnlyList<StockLine> LowStock(
        IEnumerable<Product> source,
        int threshold)
    {
        return source
            .Where(p => p.Quantity < threshold)
            .OrderBy(p => p.Quantity)
            .ThenBy(p => p.Sku, StringComparer.Ordinal)
            .Select(p => new StockLine(
                p.Sku,
                p.Name,
                p.Quantity,
                p.Unit))
            .ToList();
    }

    // 2. Топ-N товарів за кількістю
    public static IReadOnlyList<StockLine> TopByQuantity(
        IEnumerable<Product> source,
        int n)
    {
        return source
            .OrderByDescending(p => p.Quantity)
            .ThenBy(p => p.Sku, StringComparer.Ordinal)
            .Take(n)
            .Select(p => new StockLine(
                p.Sku,
                p.Name,
                p.Quantity,
                p.Unit))
            .ToList();
    }

    // 3. Групування за одиницями вимірювання
    public static IReadOnlyList<GroupSummary> ByUnit(
        IEnumerable<Product> source)
    {
        return source
            .GroupBy(
                p => p.Unit,
                StringComparer.OrdinalIgnoreCase)
            .Select(g => new GroupSummary(
                g.Key,
                g.Count(),
                g.Sum(p => p.Quantity)))
            .OrderByDescending(s => s.TotalQuantity)
            .ToList();
    }

    // 4. Загальна статистика
    public static (
        int Items,
        int Total,
        double Average,
        int Max) Totals(IEnumerable<Product> source)
    {
        var list = source.ToList();

        return list.Count == 0
            ? (0, 0, 0d, 0)
            : (
                list.Count,
                list.Sum(p => p.Quantity),
                list.Average(p => p.Quantity),
                list.Max(p => p.Quantity)
            );
    }

    // 5. Демонстрація відкладеного виконання LINQ
    public static (
        int Before,
        int After,
        int SnapshotCount) DeferredExecution(
            IEnumerable<Product> source)
    {
        // Створюємо локальний список
        var list = source.ToList();

        // Створюємо запит без негайного виконання
        var query = list.Where(p => p.Quantity < 10);

        // Перше виконання запиту
        int before = query.Count();

        // Додаємо новий товар
        list.Add(
            Product.Create(
                "tmp00001",
                "SKU-999",
                "Скоба",
                "шт",
                3));

        // Повторне виконання запиту
        int after = query.Count();

        // Фіксуємо результат
        var snapshot = query.ToList();

        // Очищаємо початковий список
        list.Clear();

        // Повертаємо результати
        return (before, after, snapshot.Count);
    }
}
