using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> where T : Component
{
    private List<T> pool;
    private GameObject prefab;
    private Transform parent;

    public GenericObjectPool(GameObject prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        pool = new List<T>();

        for (int i = 0; i < initialSize; i++)
        {
            CreateObject();
        }
    }

    private T CreateObject()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"Prefab {prefab.name} does not have component {typeof(T).Name}");
            Object.Destroy(obj);
            return null;
        }
        obj.SetActive(false);
        pool.Add(component);
        return component;
    }

    public T GetObject()
    {
        foreach (var obj in pool)
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
        obj.gameObject.SetActive(false);
        obj.transform.position = Vector3.zero;
    }

    public IReadOnlyList<T> GetPooledObjects()
    {
        return pool.AsReadOnly();
    }
}