namespace td.features.eventBus
{
    public interface IEvent
    {
    }

    // Базовый интерфейс для слушателей событий
    public interface IBaseEventReceiver
    {
        public UniqueId Id { get; }
    }

    // Интерфейс для параметризированных слушателей событий
    public interface IEventReceiver<in T> : IBaseEventReceiver where T : struct, IEvent
    {
        void OnEvent(T @event);
    }
}