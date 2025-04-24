namespace SothbeysKillerApi.Services;

public class CustomCache
{
    private Dictionary<object, object> _entries = new();

    public T? Get<T>(object key)
    {
        if (_entries.TryGetValue(key, out var entry))
        {
            return (T)entry;
        }

        return default;
    }
}