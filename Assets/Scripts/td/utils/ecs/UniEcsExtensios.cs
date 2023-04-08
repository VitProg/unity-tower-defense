using System;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UniLeo;
using UnityEngine;

namespace td.utils.ecs
{
    public static class UniEcsExtensios
    {
        public static int ConvertToEntity(this EcsWorld world, GameObject gameObject)
        {
            var convertable = gameObject.transform.GetComponent<ConvertToEntity>();

            if (!convertable)
            {
                throw new NullReferenceException();
            }
            
            convertable.Convert(world);
            convertable.TryGetEntity(out var entity);
            
            return entity;
        }
    }
}