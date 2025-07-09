using System;
using System.Collections.Generic;

namespace ProyectSecret.Events
{
    /// <summary>
    /// Event Bus global para publicar y suscribirse a eventos de juego de forma desacoplada.
    /// </summary>
    public class GameEventBus
    {
        private static GameEventBus _instance;
        public static GameEventBus Instance => _instance ?? (_instance = new GameEventBus());

        private readonly Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();

<<<<<<< Updated upstream:Assets/Scripts/Events/GameEventBus.cs
=======
        /// <summary>
        /// Evento público para notificar la publicación de cualquier evento (útil para logging, debug, UI global, etc).
        /// </summary>
        public event Action<object> OnAnyEventPublished;

>>>>>>> Stashed changes:Assets_dst/Scripts/Events/GameEventBus.cs
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
<<<<<<< Updated upstream:Assets/Scripts/Events/GameEventBus.cs
=======
            OnAnyEventPublished?.Invoke(evt);
>>>>>>> Stashed changes:Assets_dst/Scripts/Events/GameEventBus.cs
        }
    }
}
