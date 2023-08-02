using Leopotam.EcsLite.Di;
using td.features._common;
using td.features._common.components;
using td.features._common.flags;
using td.features.dragNDrop.components;
using td.features.dragNDrop.events;
using td.features.dragNDrop.evets;
using td.features.dragNDrop.flags;
using UnityEngine;

namespace td.features.dragNDrop
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DragNDrop_Pools
    {
        public readonly EcsPoolInject<DragEndEvent> dragEndEventPool = default;
        public readonly EcsPoolInject<IsDragging> isDraggingPool = default;
        public readonly EcsPoolInject<DraggingStartedData> draggingStartedDataPool = default;
        public readonly EcsPoolInject<IsRollbackDragging> isRollbackDraggingPool = default;
        public readonly EcsPoolInject<IsSmoothDragging> isSmoothDraggingPool = default;
        public readonly EcsPoolInject<IsUnableToDrop> isUnableToDropPool = default;
        public readonly EcsPoolInject<DragStartEvent> dragStartEventPool = default;
        public readonly EcsPoolInject<DragRollbackEvent> dragRollbackEventPool = default;

        public readonly EcsFilterInject<Inc<IsDragging, Ref<GameObject>>, ExcludeNotAlive> dragedFilter = default;
        
        public readonly EcsFilterInject<Inc<IsDragging, DraggingStartedData, Ref<GameObject>>, Exc<IsDestroyed>> dndFilter = default;
        public readonly EcsFilterInject<Inc<IsTargetReached, IsRollbackDragging, DraggingStartedData, Ref<GameObject>>, Exc<IsDestroyed>> rollbackFinishedFilter = default;

    }
}