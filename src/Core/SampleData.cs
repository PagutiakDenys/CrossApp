using Core.Dto;
using Core.Domain;
using Core.Import;

namespace Core;

public static class SampleData
{
    public static List<Product> Products()
    {
        string path = Path.Combine(
    AppContext.BaseDirectory,
    "data",
    "sample.csv"
);

        var imported = MixedImporter.Load(path);

        return imported
            .OfType<ProductDto>()
            .Select(Product.FromDto)
            .ToList();
    }
}