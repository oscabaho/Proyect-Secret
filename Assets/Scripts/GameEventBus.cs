using System;
using System.Collections.Generic;

/// <summary>
/// Event Bus global para publicar y suscribirse a eventos de juego de forma desacoplada.
/// </summary>
public class GameEventBus
{
    private static GameEventBus _instance;
    public static GameEventBus Instance => _instance ?? (_instance = new GameEventBus());

    private readonly Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();

    public void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (eventTable.TryGetValue(type, out var del))
            eventTable[type] = Delegate.Combine(del, handler);
        else
            eventTable[type] = handler;
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (eventTable.TryGetValue(type, out var del))
        {
            var currentDel = Delegate.Remove(del, handler);
            if (currentDel == null)
                eventTable.Remove(type);
            else
                eventTable[type] = currentDel;
        }
    }

    public void Publish<T>(T evt)
    {
        var type = typeof(T);
        if (eventTable.TryGetValue(type, out var del))
        {
            var callback = del as Action<T>;
            callback?.Invoke(evt);
        }
    }
}
