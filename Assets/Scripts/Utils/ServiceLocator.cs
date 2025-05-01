using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoSingleton<ServiceLocator>
{
    private Dictionary<Type, object> services;
    private static bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        if (services == null)
        {
            services = new Dictionary<Type, object>();
        }
        isInitialized = true;
        Debug.Log("ServiceLocator initialized successfully");
    }

    private void EnsureInitialized()
    {
        if (!isInitialized)
        {
            if (services == null)
            {
                services = new Dictionary<Type, object>();
            }
            isInitialized = true;
            Debug.Log("ServiceLocator manually initialized");
        }
    }

    public void RegisterService<T>(T service)
    {
        EnsureInitialized();
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
        EnsureInitialized();
        Type type = typeof(T);
        if (services.TryGetValue(type, out object service))
        {
            return (T)service;
        }
        throw new InvalidOperationException($"Service not found: {type.Name}. Ensure it is registered.");
    }

    public void RemoveService<T>()
    {
        EnsureInitialized();
        Type type = typeof(T);
        if (services.ContainsKey(type))
        {
            services.Remove(type);
            Debug.Log($"Service removed: {type.Name}");
        }
    }
}