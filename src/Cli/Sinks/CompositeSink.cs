using Core.Events;

public sealed class CompositeSink : INotificationSink
{
    private readonly INotificationSink[] _sinks;

    public CompositeSink(params INotificationSink[] sinks) => _sinks = sinks;

    public void Notify(CatalogChangedEventArgs e)
    {
        foreach (var sink in _sinks)
            sink.Notify(e);
    }
}