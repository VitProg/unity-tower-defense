using System;

namespace td.features.eventBus
{
    public record UniqueId
    {
        private string Id => id ??= Guid.NewGuid().ToString();
        private string id;

        public static implicit operator string(UniqueId uniqueId) => uniqueId.Id;
    }
}