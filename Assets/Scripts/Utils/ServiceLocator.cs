using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoSingleton<ServiceLocator>
{
    private Dictionary<Type, object> services;

    protected override void Awake()
    {
        base.Awake();
        services = new Dictionary<Type, object>();
    }

    public void RegisterService<T>(T service)
    {
        if (service == null)
        {
            Debug.LogError($"cannot register null service for {typeof(T).Name}.");
            return;
        }

        Type type = typeof(T);
        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"Service {type.Name} already registered. Overwriting.");
        }
        services[type] = service;
        Debug.Log($"Service registered: {type.Name}");
    }

    public T GetService<T>()
    {
        Type type = typeof(T);
        if (services.TryGetValue(type, out object service))
        {
            return (T)service;
        }
        Debug.LogError($"Service not found: {type.Name}");
        return default;
    }

    public void RemoveService<T>()
    {
        Type type = typeof(T);
        
    }
}