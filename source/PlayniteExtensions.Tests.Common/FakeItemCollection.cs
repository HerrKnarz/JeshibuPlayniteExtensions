using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlayniteExtensions.Tests.Common;

internal class FakeItemCollection<T> : IItemCollection<T> where T : DatabaseObject, new()
{
    private Dictionary<Guid, T> _items = [];

    public T this[Guid id]
    {
        get => _items[id];
        set => _items[id] = value;
    }

    public GameDatabaseCollection CollectionType => new T() switch
    {
        AgeRating _ => GameDatabaseCollection.AgeRatings,
        //TODO: implement the rest if this matters at all
        _ => GameDatabaseCollection.Uknown,
    };

    public int Count => _items.Count;

    public bool IsReadOnly => throw new NotImplementedException();

    public event EventHandler<ItemCollectionChangedEventArgs<T>> ItemCollectionChanged;
    public event EventHandler<ItemUpdatedEventArgs<T>> ItemUpdated;

    public T Add(string itemName)
    {
        var newItem = new T { Name = itemName };
        _items[newItem.Id] = newItem;
        return newItem;
    }

    public T Add(string itemName, Func<T, string, bool> existingComparer)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> Add(List<string> items)
    {
        return items.Select(Add);
    }

    public T Add(MetadataProperty property)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> Add(IEnumerable<MetadataProperty> properties)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> Add(List<string> items, Func<T, string, bool> existingComparer)
    {
        throw new NotImplementedException();
    }

    public void Add(IEnumerable<T> items)
    {
        foreach (var item in items)
            Add(item);
    }

    public void Add(T item)
    {
        if (item == null)
            return;

        _items.Add(item.Id, item);
    }

    public void BeginBufferUpdate()
    {
    }

    public IDisposable BufferedUpdate()
    {
        throw new NotImplementedException();
    }

    public void Clear() => _items.Clear();

    public bool Contains(T item) => _items.Values.Contains(item);

    public bool ContainsItem(Guid id) => _items.ContainsKey(id);

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }

    public void EndBufferUpdate()
    {
    }

    public T Get(Guid id)
    {
        return _items[id];
    }

    public List<T> Get(IList<Guid> ids)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> GetClone()
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator() => _items.Values.GetEnumerator();

    public bool Remove(Guid id) => _items.Remove(id);

    public bool Remove(IEnumerable<T> items) => items.All(Remove);

    public bool Remove(T item) => item != null && _items.Remove(item.Id);

    public void Update(T item)
    {
    }

    public void Update(IEnumerable<T> items)
    {
    }

    IEnumerator IEnumerable.GetEnumerator() => _items.Values.GetEnumerator();
}
