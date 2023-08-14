using td.features.eventBus.types;

namespace td.features.state
{
    public interface IStateChangedEvent : IUniqueEvent
    {
        bool IsEmpty();
        void Clear();
        void All();
    }
}