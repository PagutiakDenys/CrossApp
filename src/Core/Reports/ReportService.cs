
using Core.Domain;
using Core.Extensions;

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

    // 2. ТОП товарів за кількістю
    public static IReadOnlyList<StockLine> TopByQuantity(
        IEnumerable<Product> source,
        int n)
    {
        return source
            .TopBy(p => p.Quantity, n)
            .Select(p => new StockLine(
                p.Sku,
                p.Name,
                p.Quantity,
                p.Unit))
            .ToList();
    }

    // 3. Групування за одиницями вимірювання (method syntax)
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
            .OrderByDescending(g => g.TotalQuantity)
            .ToList();
    }

    // 4. Загальна статистика
    public static (
        int Items,
        int Total,
        double Average,
        int Max
    ) Totals(IEnumerable<Product> source)
    {
        var list = source.ToList();

        if (list.Count == 0)
            return (0, 0, 0d, 0);

        return (
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
        int SnapshotCount
    ) DeferredExecution(IEnumerable<Product> source)
    {
        var list = source.ToList();

        var query = list.Where(p => p.Quantity < 10);

        int before = query.Count();

        list.Add(Product.Create(
            "tmp00001",
            "SKU-999",
            "Скоба",
            "шт",
            3));

        int after = query.Count();

        var snapshot = query.ToList();

        list.Clear();

        return (
            before,
            after,
            snapshot.Count
        );
    }

    // 6. ТОП груп за загальною кількістю
    public static IReadOnlyList<GroupSummary> TopGroups(
        IEnumerable<Product> source,
        int n)
    {
        return source
            .GroupBy(
                p => p.Unit,
                StringComparer.OrdinalIgnoreCase)
            .Select(g => new GroupSummary(
                g.Key,
                g.Count(),
                g.Sum(p => p.Quantity)))
            .TopBy(g => g.TotalQuantity, n);
    }

    // 7. Групування за допомогою query syntax
    public static IReadOnlyList<GroupSummary> ByUnitQuery(
        IEnumerable<Product> source)
    {
        var query =
            from p in source
            group p by p.Unit into g
            orderby g.Sum(p => p.Quantity) descending
            select new GroupSummary(
                g.Key,
                g.Count(),
                g.Sum(p => p.Quantity));

        return query.ToList();
    }

    // 8. Об'єднання товарів та переміщень
    public static IReadOnlyList<JoinedStockLine> JoinMovements(
        IEnumerable<Product> products,
        IEnumerable<StockMovement> movements)
    {
        return products
            .GroupJoin(
                movements,
                product => product.Id,
                movement => movement.ProductId,
                (product, productMovements) =>
                    new JoinedStockLine(
                        product.Sku,
                        product.Name,
                        product.Unit,
                        product.Quantity,

                        productMovements
                            .Where(m => m.Operation == "IN")
                            .Sum(m => m.Quantity),

                        productMovements
                            .Where(m => m.Operation == "OUT")
                            .Sum(m => m.Quantity),

                        productMovements.Count()
                    ))
            .OrderBy(
                p => p.Sku,
                StringComparer.Ordinal)
            .ToList();
    }

    // 9. Створення демонстраційних переміщень
    public static IReadOnlyList<StockMovement> CreateSampleMovements(
        IEnumerable<Product> products)
    {
        var result = new List<StockMovement>();

        foreach (var product in products)
        {
            result.Add(new StockMovement(
                product.Id,
                "IN",
                20));

            result.Add(new StockMovement(
                product.Id,
                "OUT",
                5));
        }

        return result;
    }

    // 10. Порівняння продуктивності LINQ
    public static (
        long WithIntermediateTicks,
        long WithoutIntermediateTicks,
        int WithCount,
        int WithoutCount
    ) Benchmark()
    {
        var products = Enumerable.Range(1, 100_000)
            .Select(i => Product.Create(
                $"B-{i}",
                $"SKU-{i:D6}",
                $"Товар {i}",
                "шт",
                i % 500))
            .ToList();

        // Варіант із проміжним ToList()
        var stopwatch =
            System.Diagnostics.Stopwatch.StartNew();

        var withIntermediate = products
            .Where(p => p.Quantity > 100)
            .ToList()
            .OrderByDescending(p => p.Quantity)
            .Take(100)
            .ToList();

        stopwatch.Stop();

        long withTicks = stopwatch.ElapsedTicks;

        // Варіант без проміжного ToList()
        stopwatch.Restart();

        var withoutIntermediate = products
            .Where(p => p.Quantity > 100)
            .OrderByDescending(p => p.Quantity)
            .Take(100)
            .ToList();

        stopwatch.Stop();

        long withoutTicks = stopwatch.ElapsedTicks;

        return (
            withTicks,
            withoutTicks,
            withIntermediate.Count,
            withoutIntermediate.Count
        );
    }
}
