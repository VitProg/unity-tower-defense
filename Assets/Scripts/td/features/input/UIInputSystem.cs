using Leopotam.EcsLite;
using Leopotam.EcsLite.Unity.Ugui;
using td.components.attributes;
using td.components.behaviors;
using td.components.links;
using td.states;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.Scripting;

namespace td.features.input
{
    public class UIInputSystem : EcsUguiCallbackSystem
    {
        [EcsInject] private LevelState levelState;
        [EcsWorld] private EcsWorld world;
        [EcsWorld(Constants.Worlds.Outer)] private EcsWorld outerWorld;
        // private IEcsSystems systems;
        
        [EcsUguiNamed(Constants.UI.AddTowerButton)] private GameObject addTowerButton;

        private Vector2 addTowerButtonStartPosition;

        public override void Run(IEcsSystems systems)
        {
            // this.systems = systems;
            base.Run(systems);
        }

        [Preserve]
        [EcsUguiClickEvent(Constants.UI.AddTowerButton, Constants.Worlds.UI)]
        private void OnAddTowerClick(in EcsUguiClickEvent e)
        {
            levelState.IsBuildingProcess = true;
            Debug.Log("!!! CLICKED !!!");
        }
        
        [Preserve]
        [EcsUguiDragStartEvent(Constants.UI.AddTowerButton, Constants.Worlds.UI)]
        private void OnAddTowerDragStart(in EcsUguiDragStartEvent e)
        {
            Debug.Log("!!! DragStart !!!");
            addTowerButton.transform.position = e.Position;
            Debug.Log(e.Position);
        }
        
        [Preserve]
        [EcsUguiDragMoveEvent(Constants.UI.AddTowerButton, Constants.Worlds.UI)]
        private void OnAddTowerDragMove(in EcsUguiDragMoveEvent e)
        {
            Debug.Log("!!! DragMove !!!");
            addTowerButton.transform.position = e.Position;
            Debug.Log(e.Position);
        }
        
        [Preserve]
        [EcsUguiDragEndEvent(Constants.UI.AddTowerButton, Constants.Worlds.UI)]
        private void OnAddTowerDragEnd(in EcsUguiDragEndEvent e)
        {
            levelState.IsBuildingProcess = true;
            Debug.Log("!!! DragEnd !!!");
            
            // var addTowerButtonEntity = world.NewEntity();
            // world.AddComponent(addTowerButtonEntity, new GameObjectLink()
            // {
            //     gameObject = addTowerButton
            // });
            // world.AddComponent<MoveToTarget>(addTowerButtonEntity);
            // world.AddComponent(addTowerButtonEntity, new Target()
            // {
            //     target = addTowerButtonStartPosition,
            // });
            //
            // world.GetPool<MoveToTarget>().Add(addTowerButtonEntity);
            //todo
            
            Debug.Log(e.Position);
        }

    }
}