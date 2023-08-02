using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;
using td.features._common;
using td.features.dragNDrop.events;
using td.features.dragNDrop.flags;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features.dragNDrop
{
    public class DragNDrop_Service
    {
        private readonly EcsInject<DragNDrop_Pools> pools;
        private readonly EcsInject<SharedData> shared;
        private readonly EcsInject<Common_Service> common;
        private readonly EcsWorldInject world;

        private readonly GameObject canvasDragLayer;

        public DragNDrop_Service()
        {
            //todo move to DI or sharedData
            canvasDragLayer = GameObject.FindGameObjectWithTag(Constants.Tags.CanvasDragLayer);
        }

        public bool IsAnyoneDragged() => pools.Value.dragedFilter.Value.GetEntitiesCount() > 0;

        public bool IsUnableToDrop(int entity) => pools.Value.isUnableToDropPool.Value.Has(entity);
        public bool CanDrop(int entity) => !IsUnableToDrop(entity);

        public void SetIsUnableToDrop(int entity, bool value)
        {
            if (value) pools.Value.isUnableToDropPool.Value.SafeAdd(entity);
            else pools.Value.isUnableToDropPool.Value.SafeDel(entity);
        }

        public void SetCanDrop(int entity, bool value) => SetIsUnableToDrop(entity, !value);

        public bool IsRollback(int entity) => pools.Value.isRollbackDraggingPool.Value.Has(entity);

        public void SetIsRollback(int entity, bool value, DragMode mode = DragMode.Camera)
        {
            if (value) pools.Value.isRollbackDraggingPool.Value.GetOrAdd(entity).mode = mode;
            else pools.Value.isRollbackDraggingPool.Value.SafeDel(entity);
        }

        public ref IsRollbackDragging GetIsRollback(int entity) =>
            ref pools.Value.isRollbackDraggingPool.Value.Get(entity);

        public bool IsSmooth(int entity) => pools.Value.isSmoothDraggingPool.Value.Has(entity);
        public ref IsSmoothDragging GetIsSmooth(int entity) => ref pools.Value.isSmoothDraggingPool.Value.GetOrAdd(entity);
        public void RemoveIsSmooth(int entity) => pools.Value.isSmoothDraggingPool.Value.SafeDel(entity);

        public bool IsDragging(int entity) => pools.Value.isDraggingPool.Value.Has(entity);
        public void RemoveIsDragging(int entity)
        {
            pools.Value.isDraggingPool.Value.SafeDel(entity);
            RemoveIsSmooth(entity);
        }
        public ref IsDragging GetIsDragging(int entity) => ref pools.Value.isDraggingPool.Value.GetOrAdd(entity);

        public void BeginDragCameraSpace(
            int entity,
            bool smoothDragging = false,
            float smoothSpeed = Constants.UI.DragNDrop.SmoothSpeed
        )
        {
            if (!common.Value.HasGameObject(entity, true)) return;
            var transform = common.Value.GetGOTransform(entity);

            ref var isDragging = ref GetIsDragging(entity);
            isDragging.mode = DragMode.Camera;
            isDragging.isGridSnapping = false;
            isDragging.state = Input.GetMouseButtonDown(0) ? IsDraggingState.None : IsDraggingState.Down;

            ref var draggingStartedData = ref pools.Value.draggingStartedDataPool.Value.GetOrAdd(entity);
            draggingStartedData.startedPosition = transform.position;
            draggingStartedData.startedTime = Time.timeSinceLevelLoadAsDouble;
            draggingStartedData.parentContainer = transform.parent.transform;

            pools.Value.dragStartEventPool.Value.GetOrAdd(entity).mode = DragMode.Camera;

            var cursorPosition = CameraUtils.ToWorldPoint(shared.Value.canvasCamera, Input.mousePosition);

            transform.position = cursorPosition;
            transform.SetParent(canvasDragLayer.transform);

            if (smoothDragging)
            {
                var hasLinearMovement = common.Value.HasMovement(entity);

                ref var isSmoothDragging = ref GetIsSmooth(entity);
                isSmoothDragging.speed = smoothSpeed;
                isSmoothDragging.removeLinearMovementWhenFinished = !hasLinearMovement;

                ref var linearMovementToTarget = ref common.Value.GetMovement(entity);
                linearMovementToTarget.target = transform.position;
                linearMovementToTarget.gapSqr = Constants.DefaultGapSqr;
                linearMovementToTarget.speed = Constants.UI.DragNDrop.SmoothSpeed;
            }
        }

        public void BeginDragWorldSpace(
            int entity,
            bool snapToGrid = false,
            bool smoothDragging = false,
            float smoothSpeed = Constants.UI.DragNDrop.SmoothSpeed,
            bool moveToUpperLayer = true
        )
        {
            if (!common.Value.HasGameObject(entity, true)) return;
            var transform = common.Value.GetGOTransform(entity);

            var sortingLayerChangeable = transform.GetComponent<ISortingLayerChangeable>();

            ref var isDragging = ref GetIsDragging(entity);
            isDragging.mode = DragMode.World;
            // isDragging.startedPosition = refGameObject.reference.transform.position;
            // isDragging.startedTime = Time.timeSinceLevelLoadAsDouble;
            isDragging.isGridSnapping = snapToGrid;
            isDragging.state = Input.GetMouseButtonDown(0) ? IsDraggingState.None : IsDraggingState.Down;
            // isDragging.startedLayer = sortingLayerChangeable?.sortigLayer ?? Constants.Layers.L3_Buildings;

            ref var draggingStartedData = ref pools.Value.draggingStartedDataPool.Value.GetOrAdd(entity);
            draggingStartedData.startedPosition = transform.position;
            draggingStartedData.startedTime = Time.timeSinceLevelLoadAsDouble;
            draggingStartedData.startedLayer = sortingLayerChangeable?.sortigLayer ?? Constants.Layers.L3_Buildings;
            draggingStartedData.parentContainer = transform.parent.transform;

            pools.Value.dragStartEventPool.Value.GetOrAdd(entity).mode = DragMode.World;

            if (moveToUpperLayer && sortingLayerChangeable != null)
            {
                sortingLayerChangeable.sortigLayer = Constants.Layers.L11_Draggable;
            }

            // Debug.Log($"> DnD: state={isDragging.state.ToString()}; mb={(Input.GetMouseButtonDown(0) ? "DOWN" : "UP")}");

            if (smoothDragging)
            {
                var hasLinearMovement = common.Value.HasMovement(entity);

                ref var isSmoothDragging = ref GetIsSmooth(entity);
                isSmoothDragging.speed = smoothSpeed;
                isSmoothDragging.removeLinearMovementWhenFinished = !hasLinearMovement;

                ref var linearMovementToTarget = ref common.Value.GetMovement(entity);
                linearMovementToTarget.target = transform.position;
                linearMovementToTarget.gapSqr = Constants.DefaultGapSqr;
                linearMovementToTarget.speed = Constants.UI.DragNDrop.SmoothSpeed;
            }
        }

        public void Clear(int entity)
        {
            ref var draggingStartedData = ref pools.Value.draggingStartedDataPool.Value.Get(entity);
            var go = common.Value.GetGameObject(entity);

            if (go)
            {
                go.transform.position = draggingStartedData.startedPosition;
                go.transform.SetParent(draggingStartedData.parentContainer);
            }

            SetIsRollback(entity, false);
            RemoveIsDragging(entity);
            common.Value.RemoveMovement(entity);
            pools.Value.draggingStartedDataPool.Value.SafeDel(entity);
            pools.Value.dragRollbackEventPool.Value.SafeDel(entity);
            pools.Value.dragEndEventPool.Value.SafeDel(entity);
        }


        // ToDo: Add method for Rollback
    }
}