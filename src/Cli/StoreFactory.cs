using Core;
using Core.Abstractions;
using Core.Storage;

public static class StoreFactory
{
    public static ICatalogStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");

        if (useFile)
        {
            string projectRoot = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "../../../../.."));

            string dataPath = Path.Combine(
                projectRoot, "data", "catalog.json");

            return new FileCatalogStore(dataPath);
        }

        return new InMemoryCatalogStore(SampleData.Products());
    }
}