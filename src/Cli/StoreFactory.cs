
using Core;
using Core.Abstractions;
using Core.Storage;

public static class StoreFactory
{
    public static ICatalogStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");
        bool useCaching = args.Contains("--caching");

        ICatalogStore store;

        if (useFile)
        {
            string projectRoot = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "../../../../.."));

            string dataPath = Path.Combine(
                projectRoot, "data", "catalog.json");

            store = new FileCatalogStore(dataPath);
        }
        else
        {
            store = new InMemoryCatalogStore(SampleData.Products());
        }

        if (useCaching)
        {
            store = new CachingCatalogStore(store);
        }

        return store;
    }
}
