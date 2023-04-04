using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Shapes2D;
using td.components.flags;
using td.components.links;
using UnityEngine;

namespace td.features.levels
{
    public class BuildingsInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<IsTower, GameObjectLink>> entities = default;

        public void Init(IEcsSystems systems)
        {
            foreach (var entity in entities.Value)
            {
                var tower = entities.Pools.Inc1.Get(entity);
                var towerGameObject = entities.Pools.Inc2.Get(entity);
                
                var radius = towerGameObject.gameObject.transform.Find("radius");
                radius.gameObject.hideFlags = HideFlags.None;
                radius.localScale = new Vector3(tower.radius, tower.radius, tower.radius) * 1.3f;
                var shape = radius.GetComponent<Shape>();
                if (shape)
                {
                    //todo
                }
            }
            var parent = GameObject.FindGameObjectWithTag(Constants.Tags.BuildingsContainer);

            var buildings = GameObject.FindGameObjectsWithTag(Constants.Tags.Building);

            foreach (var build in buildings)
            {
                
            }
        }
    }
}