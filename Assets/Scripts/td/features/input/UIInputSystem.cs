using Leopotam.EcsLite;
using Leopotam.EcsLite.Unity.Ugui;
using td.components.behaviors;
using td.components.events;
using td.components.flags;
using td.states;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Scripting;

namespace td.features.input
{
    public class UIInputSystem : EcsUguiCallbackSystem, IEcsInitSystem
    {
        [EcsInject] private LevelState levelState;
        [EcsWorld] private EcsWorld world;
        [EcsWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;
        
        [EcsUguiNamed(Constants.UI.Components.AddTowerButton)] private GameObject addTowerButton;

        private IEcsSystems systems;
        private GameObject buildingsContainer;

        [Preserve]
        [EcsUguiDownEvent(Constants.UI.Components.AddTowerButton, Constants.Worlds.UI)]
        private void OnAddTowerDown(in EcsUguiDownEvent e)
        {
            if (buildingsContainer == null)
            {
                buildingsContainer = GameObject.FindGameObjectWithTag(Constants.Tags.BuildingsContainer);
            }
            
            var position = InputToWorldPosition(e.Position);
            var prefab = Resources.Load<GameObject>("Prefabs/buildings/tower_v1");
            var gameObject = Object.Instantiate(prefab, position, Quaternion.identity, buildingsContainer.transform);
            var entity = world.ConvertToEntity(gameObject);
            world.AddComponent<IsDragging>(entity).startedTime = Time.timeSinceLevelLoadAsDouble;
            systems.SendOuter<DragStartEvent>();

            if (Constants.UI.DragNDrop.Smooth)
            {
                world.AddComponent(entity, new LinearMovementToTarget()
                {
                    gap = Constants.DefaultGap,
                    speed = Constants.UI.DragNDrop.SmoothSpeed,
                    target = position
                });
            }

            // todo создать башну в сцене, привязав координаты к мыши
            // todo отправить событие DragStartEvent
            // todo добавить на ентити флаг IsDragging
        }
        
        // todo для всех ентити с флагом IsDragging менять ее координаты на координаты курсора со снапингом в сетки

        //     // todo drug end
        //     // todo отправить событие DragEndEvent
        //     // todo удалить с ентити флаг IsDragging
        //     // todo произвести инициализацию башни в мире

        //todo move to utils static class
        public static Vector3 InputToWorldPosition(Vector2 inputPos) {
            Vector3 pos = new Vector3(inputPos.x, inputPos.y, 
                -Camera.main.transform.position.z);
            return Camera.main.ScreenToWorldPoint(pos);
        }

        public void Init(IEcsSystems systems)
        {
            this.systems = systems;
        }
    }
}