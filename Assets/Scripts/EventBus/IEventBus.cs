using System;

public interface IEventBus
{
    void Publish<TEvent>(TEvent evt);
    void Subscribe<TEvent>(Action<TEvent> handler);
    void Unsubscribe<TEvent>(Action<TEvent> handler);
}