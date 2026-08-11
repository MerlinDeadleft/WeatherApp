namespace WeatherApp.Core;

public interface IMessenger
{
    void Subscribe<TMessage>(Action<TMessage> action);
    void Publish<TMessage>(TMessage message);
}

public class Messenger : IMessenger
{
    private Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();

    public void Subscribe<TMessage>(Action<TMessage> action)
    {
        var messageType = typeof(TMessage);

        if (!subscribers.ContainsKey(messageType))
        {
            subscribers.Add(messageType, new List<Delegate>());
        }
        
        subscribers[messageType].Add(action);
    }

    public void Publish<TMessage>(TMessage message)
    {
        var messageType = typeof(TMessage);
        
        if(!subscribers.ContainsKey(messageType)) return;
        
        var handlers = subscribers[messageType].ToList();

        foreach (var handler in handlers)
        {
            ((Action<TMessage>)handler).Invoke(message);
        }
    }
}