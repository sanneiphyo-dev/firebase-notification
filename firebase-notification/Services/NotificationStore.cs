using System.Text.Json;
using FirebaseNotification.Features.Notification.Models;

namespace FirebaseNotification.Services;

public sealed class NotificationStore
{
    private static readonly Lock FileLock = new();
    private readonly string _filePath;
    private readonly List<NotificationModel> _items = [];

    public NotificationStore(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Config", "notifications-history.json");
        Load();
    }

    public (int TotalCount, List<NotificationModel> Items) GetPaged(int pageNo, int pageSize, string? keyword)
    {
        lock (FileLock)
        {
            var query = _items.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.Target.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.OrderByDescending(x => x.CreatedDateTime).ToList();
            int total = list.Count;
            var paged = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            return (total, paged);
        }
    }

    public NotificationModel? GetById(string id)
    {
        lock (FileLock)
        {
            return _items.FirstOrDefault(x => x.NotificationId == id);
        }
    }

    public void Add(NotificationModel model)
    {
        lock (FileLock)
        {
            _items.Insert(0, model);
            Save();
        }
    }

    public bool Delete(string id)
    {
        lock (FileLock)
        {
            int removed = _items.RemoveAll(x => x.NotificationId == id);
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
                var loaded = JsonSerializer.Deserialize<List<NotificationModel>>(json);
                if (loaded is not null)
                {
                    _items.Clear();
                    _items.AddRange(loaded);
                }
            }
            catch
            {
                // Fallback to empty
            }
        }
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            string json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // Silently ignore disk write issues
        }
    }
}
