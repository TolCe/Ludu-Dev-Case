using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T _prefab;
    private Transform _parentTransform;
    private List<T> _pool;
    private List<T> _reservedItems;

    public ObjectPool(T prefab, int initialSize, Transform parentTransform)
    {
        this._prefab = prefab;
        this._parentTransform = parentTransform;

        this._pool = new List<T>(initialSize);
        this._reservedItems = new List<T>();

        for (int i = 0; i < initialSize; i++)
        {
            T newObj = Object.Instantiate(prefab, parentTransform);
            newObj.gameObject.SetActive(false);
            _pool.Add(newObj);
        }
    }

    public T Get()
    {
        if (_pool.Count > 0)
        {
            T item = _pool[0];

            _reservedItems.Add(item);

            _pool.RemoveAt(0);

            return item;
        }

        T newObj = Object.Instantiate(_prefab, _parentTransform);
        _reservedItems.Add(newObj);
        return newObj;
    }

    public List<T> GetActiveObjects()
    {
        return _reservedItems.ToList();
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _reservedItems.Remove(obj);
        _pool.Insert(0, obj);
        obj.transform.SetParent(_parentTransform);
    }

    public void ReturnAll()
    {
        _reservedItems.ForEach(e =>
        {
            e.gameObject.SetActive(false);
            _pool.Insert(0, e);
        });

        _reservedItems.Clear();
    }
}
