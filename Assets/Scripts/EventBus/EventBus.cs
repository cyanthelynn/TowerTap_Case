using System;
using System.Collections.Generic;
using System.Linq;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers 
        = new Dictionary<Type, List<Delegate>>();

    public void Publish<TEvent>(TEvent evt)
    {
        var eventType = typeof(TEvent);
        if (_subscribers.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers.ToArray().Cast<Action<TEvent>>())
            {
                handler.Invoke(evt);
            }
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        var eventType = typeof(TEvent);
        if (!_subscribers.TryGetValue(eventType, out var handlers))
        {
            handlers = new List<Delegate>();
            _subscribers[eventType] = handlers;
        }
        handlers.Add(handler);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        var eventType = typeof(TEvent);
        if (_subscribers.TryGetValue(eventType, out var handlers))
        {
            handlers.Remove(handler);
            if (handlers.Count == 0)
                _subscribers.Remove(eventType);
        }
    }
}