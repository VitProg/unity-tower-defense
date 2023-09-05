using td.features.eventBus.types;

namespace td.features.state.interfaces
{
    public interface IStateChangedEvent : IUniqueEvent
    {
        bool IsEmpty();
        void Clear();
        void All();
    }
}