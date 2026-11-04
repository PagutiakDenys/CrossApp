
using Core.Dto;

namespace Core.Import;

public static class MixedImporter
{
    public static List<object> Load(string path)
    {
        var result = new List<object>();

        foreach (string line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(';');

            // Пропускаємо заголовок CSV
            if (parts[0].Equals("id", StringComparison.OrdinalIgnoreCase))
                continue;

            // Формат P;id;sku;name;unit;quantity
            if (parts.Length == 6 && parts[0] == "P")
            {
                if (int.TryParse(parts[5], out int q) &&
                    q >= 0 &&
                    !string.IsNullOrWhiteSpace(parts[1]) &&
                    !string.IsNullOrWhiteSpace(parts[2]) &&
                    !string.IsNullOrWhiteSpace(parts[3]) &&
                    !string.IsNullOrWhiteSpace(parts[4]))
                {
                    result.Add(new ProductDto(
                        parts[1].Trim(),
                        parts[2].Trim(),
                        parts[3].Trim(),
                        parts[4].Trim(),
                        q));
                }

                continue;
            }

            // Формат id;sku;name;unit;quantity
            if (parts.Length == 5)
            {
                if (int.TryParse(parts[4], out int q) &&
                    q >= 0 &&
                    !string.IsNullOrWhiteSpace(parts[0]) &&
                    !string.IsNullOrWhiteSpace(parts[1]) &&
                    !string.IsNullOrWhiteSpace(parts[2]) &&
                    !string.IsNullOrWhiteSpace(parts[3]))
                {
                    result.Add(new ProductDto(
                        parts[0].Trim(),
                        parts[1].Trim(),
                        parts[2].Trim(),
                        parts[3].Trim(),
                        q));
                }

                continue;
            }

            // Формат W;id;name
            if (parts.Length == 3 && parts[0] == "W")
            {
                result.Add(new WarehouseDto(
                    parts[1],
                    parts[2]));
            }
        }

        return result;
    }
}
