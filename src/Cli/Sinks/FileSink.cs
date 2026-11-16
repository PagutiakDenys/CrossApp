using Core.Events;

public sealed class FileSink : INotificationSink
{
    private readonly string _path;

    public FileSink(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
    }

    public void Notify(CatalogChangedEventArgs e) =>
        File.AppendAllText(
            _path,
            $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}"
            + Environment.NewLine);
}