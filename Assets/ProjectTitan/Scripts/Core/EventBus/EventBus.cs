using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Titan
{
    public static class EventBus
    {
        // private static readonly IDictionary<string, List<IEventListener>> _eventListeners = new Dictionary<string, List<IEventListener>>();
        // private void TransitionToState<T>() where T : IEnemyState
        private static readonly Dictionary<System.Type, object> _events = new();
        public delegate void EventCallback<T>(T e) where T : IEvent;
        public static void RegisterCallback<T>(EventCallback<T> eventCallback) where T : IEvent
        {
            if (!_events.ContainsKey(typeof(T)))
            {
                _events.Add(typeof(T), new List<EventCallback<T>>());
            }
            var callbacks = _events[typeof(T)] as List<EventCallback<T>>;
            callbacks.Add(eventCallback);
        }

        public static void UnregisterCallback<T>(EventCallback<T> eventCallback) where T : IEvent
        {
            if (_events.ContainsKey(typeof(T)))
            {
                var callbacks = _events[typeof(T)] as List<EventCallback<T>>;
                callbacks.Remove(eventCallback);
            }
        }

        public static void RaiseEvent<T>(T e) where T : IEvent
        {
            if (_events.ContainsKey(typeof(T)))
            {
                var callbacks = _events[typeof(T)] as List<EventCallback<T>>;
                foreach (var callback in callbacks)
                {
                    callback(e);
                }
            }
        }
    }
}
