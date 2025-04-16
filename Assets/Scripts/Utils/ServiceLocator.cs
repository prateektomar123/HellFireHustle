using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator
{
    private static ServiceLocator instance;
    private Dictionary<Type, object> services;

    private ServiceLocator()
    {
        services = new Dictionary<Type, object>();
    }

    public static ServiceLocator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ServiceLocator();
            }
            return instance;
        }
    }

    public void RegisterService<T>(T service)
    {
        if (service == null)
        {
            Debug.LogError($"Cannot register null service for type {typeof(T)}");
            return;
        }
        services[typeof(T)] = service;
        Debug.Log($"Service registered: {typeof(T)}");
    }

    public T GetService<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        Debug.LogError($"Service not found: {typeof(T)}");
        return default;
    }

    public void ClearServices()
    {
        services.Clear();
    }
}