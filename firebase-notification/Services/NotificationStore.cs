using System.Text.Json;
using FirebaseNotification.Models;

namespace FirebaseNotification.Services;

public sealed class NotificationStore
{
    private static readonly Lock FileLock = new();
    private readonly string _filePath;
    private readonly List<NotificationRecord> _items = [];

    public NotificationStore()
    {
        string dir = AppContext.BaseDirectory;
        _filePath = Path.Combine(dir, "notifications-history.json");
        Load();
    }

    public IReadOnlyList<NotificationRecord> GetAll()
    {
        lock (FileLock)
        {
            return _items.OrderByDescending(x => x.CreatedAt).ToList();
        }
    }

    public NotificationRecord? Get(string id)
    {
        lock (FileLock)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }
    }

    public void Add(NotificationRecord record)
    {
        lock (FileLock)
        {
            _items.Insert(0, record);
            Save();
        }
    }

    public bool Delete(string id)
    {
        lock (FileLock)
        {
            int removed = _items.RemoveAll(x => x.Id == id);
            if (removed > 0)
            {
                Save();
                return true;
            }
            return false;
        }
    }

    public void Clear()
    {
        lock (FileLock)
        {
            _items.Clear();
            Save();
        }
    }

    private void Load()
    {
        lock (FileLock)
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                string json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<List<NotificationRecord>>(json);
                if (loaded is not null)
                {
                    _items.Clear();
                    _items.AddRange(loaded);
                }
            }
            catch
            {
                // Start fresh on deserialization errors
            }
        }
    }

    private void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // Ignore file write errors in transient environments
        }
    }
}
