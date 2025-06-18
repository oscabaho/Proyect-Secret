using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Service Locator global para registrar y obtener servicios del juego (inventario, eventos, etc).
/// </summary>
public class GameServices : MonoBehaviour
{
    private static GameServices _instance;
    public static GameServices Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameServices>();
                if (_instance == null)
                {
                    var go = new GameObject("GameServices");
                    _instance = go.AddComponent<GameServices>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private Dictionary<Type, object> services = new Dictionary<Type, object>();

    public void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (!services.ContainsKey(type))
            services.Add(type, service);
        else
            services[type] = service;
    }

    public T Get<T>() where T : class
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out var service))
            return service as T;
        return null;
    }
}
