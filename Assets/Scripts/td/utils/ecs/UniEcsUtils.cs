using System;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UniLeo;
using UnityEngine;

namespace td.utils.ecs
{
    public static class UniEcsUtils
    {
        public static int Convert(GameObject gameObject, EcsWorld world)
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