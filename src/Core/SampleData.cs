
using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<Product> Products()
    {
        return new List<Product>
        {
            // Метри — 4 позиції, загалом 340
            Product.Create("P-001", "SKU-001", "Кабель", "м", 120),
            Product.Create("P-002", "SKU-002", "Провід", "м", 100),
            Product.Create("P-003", "SKU-003", "Шланг", "м", 80),
            Product.Create("P-004", "SKU-004", "Труба", "м", 40),

            // Штуки — 9 позицій, загалом 187
            Product.Create("P-005", "SKU-005", "Цвяхи", "шт", 3),
            Product.Create("P-006", "SKU-006", "Гвинти", "шт", 5),
            Product.Create("P-007", "SKU-007", "Болти", "шт", 8),
            Product.Create("P-008", "SKU-008", "Гайки", "шт", 30),
            Product.Create("P-009", "SKU-009", "Шайби", "шт", 29),
            Product.Create("P-010", "SKU-010", "Цегла", "шт", 28),
            Product.Create("P-011", "SKU-011", "Плитка", "шт", 27),
            Product.Create("P-012", "SKU-012", "Дюбелі", "шт", 28),
            Product.Create("P-013", "SKU-013", "Скоби", "шт", 29),

            // Кілограми — 2 позиції, загалом 50
            Product.Create("P-014", "SKU-014", "Цемент", "кг", 30),
            Product.Create("P-015", "SKU-015", "Пісок", "кг", 20),

            // Упаковки — 2 позиції, загалом 35
            Product.Create("P-016", "SKU-016", "Клей", "уп", 20),
            Product.Create("P-017", "SKU-017", "Шпаклівка", "уп", 15)
        };
    }
}
