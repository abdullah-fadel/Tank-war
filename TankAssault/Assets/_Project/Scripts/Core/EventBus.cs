using System;
using System.Collections.Generic;

namespace TankAssault.Core
{
    /// <summary>
    /// Lightweight static event bus decoupling gameplay systems from UI/audio/analytics listeners.
    /// Usage: EventBus.Subscribe&lt;PlayerDamagedEvent&gt;(OnPlayerDamaged); EventBus.Publish(new PlayerDamagedEvent(...));
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> Listeners = new Dictionary<Type, List<Delegate>>();

        public static void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!Listeners.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                Listeners[type] = list;
            }
            list.Add(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (Listeners.TryGetValue(type, out var list))
                list.Remove(handler);
        }

        public static void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (!Listeners.TryGetValue(type, out var list)) return;

            // Copy to array to allow handlers to unsubscribe during iteration.
            var snapshot = list.ToArray();
            foreach (var d in snapshot)
                ((Action<T>)d)?.Invoke(eventData);
        }

        public static void Clear()
        {
            Listeners.Clear();
        }
    }
}
