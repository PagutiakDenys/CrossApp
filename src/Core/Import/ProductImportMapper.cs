
using Core.Domain;
using Core.Dto;

namespace Core.Import;

public sealed record ProductMappingResult(
    List<Product> Items,
    List<string> Errors);

public static class ProductImportMapper
{
    public static ProductMappingResult Map(
        ImportResult<ProductDto> importResult)
    {
        ArgumentNullException.ThrowIfNull(importResult);

        var products = new List<Product>();
        var errors = new List<string>(importResult.Errors);

        for (int i = 0; i < importResult.Items.Count; i++)
        {
            ProductDto dto = importResult.Items[i];

            try
            {
                Product product = Product.FromDto(dto);
                products.Add(product);
            }
            catch (ArgumentException ex)
            {
                errors.Add(
                    $"Запис {i + 1}: інваріант порушено — {ex.Message}");
            }
        }

        return new ProductMappingResult(products, errors);
    }
}
