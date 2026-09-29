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

            switch (parts)
            {
                case ["P", var id, var sku, var name, var unit, var quantity]
                    when int.TryParse(quantity, out int q) && q >= 0:

                    result.Add(
                        new ProductDto(
                            id,
                            sku,
                            name,
                            unit,
                            q));
                    break;

                case ["W", var id, var name]:

                    result.Add(
                        new WarehouseDto(
                            id,
                            name));
                    break;
            }
        }

        return result;
    }
}