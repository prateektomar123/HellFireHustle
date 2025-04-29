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

        Debug.Log($"Pool for {typeof(T).Name} initialized with {initialSize} objects");
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
        // Debug log to help troubleshoot
        Debug.Log($"Attempting to get object from {typeof(T).Name} pool. Pool size: {_pool.Count}");

        // First try to find an inactive object in the pool
        foreach (T obj in _pool)
        {
            if (obj != null && !obj.gameObject.activeInHierarchy)
            {
                Debug.Log($"Reusing existing inactive {typeof(T).Name} from pool");
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        // If no inactive objects found and we're below max size, create a new one
        if (_pool.Count < _maxSize)
        {
            Debug.Log($"Creating new {typeof(T).Name} for pool");
            T newObj = CreateObject();
            if (newObj != null)
            {
                newObj.gameObject.SetActive(true);
                return newObj;
            }
        }

        // If we're at max size, find the oldest object to recycle
        Debug.LogWarning($"Pool for {typeof(T).Name} reached max size ({_maxSize}). Recycling oldest active object.");
        if (_pool.Count > 0)
        {
            T oldestObject = _pool[0];
            Debug.Log($"Recycling oldest {typeof(T).Name} object");
            ReturnObject(oldestObject);
            oldestObject.gameObject.SetActive(true);
            return oldestObject;
        }

        Debug.LogError($"Pool for {typeof(T).Name} is empty and at max size. This should not happen.");
        return null;
    }

    public void ReturnObject(T obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Attempted to return null object to pool");
            return;
        }

        // Reset the object's state before returning to pool
        obj.gameObject.SetActive(false);
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Debug.Log($"Object returned to {typeof(T).Name} pool");
    }

    public IReadOnlyList<T> PooledObjects => _pool.AsReadOnly();
}