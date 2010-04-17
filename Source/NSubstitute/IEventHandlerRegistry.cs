namespace NSubstitute
{
    public interface IEventHandlerRegistry
    {
        void Add(string eventName, object handler);
        void Remove(string eventName, object handler);
    }
}