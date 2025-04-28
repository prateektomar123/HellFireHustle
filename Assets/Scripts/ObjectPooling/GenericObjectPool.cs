using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> where T : Component
{
    private readonly List<T> _pool = new();
    private GameObject _prefab;
    private Transform _parent;
    private int _maxSize;

    public GenericObjectPool(GameObject prefab, int initialSize, int maxSize, Transform parent)
    {
        if (prefab == null || !prefab.GetComponent<T>())
        {
            Debug.LogError($"Prefab {prefab?.name} is invalid or lacks {typeof(T).Name}.");
            return;
        }

        _prefab = prefab;
        _parent = parent;
        _maxSize = maxSize;

        for (int i = 0; i < initialSize; i++)
        {
            CreateObject();
        }
    }

    private T CreateObject()
    {
        if (_pool.Count >= _maxSize)
        {
            Debug.LogWarning($"Pool for {typeof(T).Name} reached max size: {_maxSize}.");
            return null;
        }

        GameObject obj = Object.Instantiate(_prefab, _parent);
        T component = obj.GetComponent<T>();
        obj.SetActive(false);
        _pool.Add(component);
        return component;
    }

    public T GetObject()
    {
        foreach (T obj in _pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        return CreateObject();
    }

    public void ReturnObject(T obj)
    {
        if (obj == null) return;
        obj.gameObject.SetActive(false);
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public IReadOnlyList<T> PooledObjects => _pool.AsReadOnly();
}