using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;

namespace td.services.ecsConverter
{
    public class EntityConverters : IHaveCostomResolve
    {
        [InjectWorld] private EcsWorld world;
        
        private Dictionary<Type, object> dictionary = new(10);

        public EntityConverters Add<TS>(IEntityConverter<TS> converter)
            where TS : struct
        {
            dictionary.Add(typeof(TS), converter);
            return this;
        }

        public IEntityConverter<TS> Get<TS>() where TS : struct
        {
            dictionary.TryGetValue(typeof(TS), out var converter);
            return (IEntityConverter<TS>)converter;
        }

        public bool ConvertForEntity<TS>(GameObject gameObject, int entity) where TS : struct
        {
            var converter = Get<TS>();

            if (converter == null)
            {
                return false;
            }
            
            if (gameObject.TryGetComponent<EcsEntity>(out var e))
            {
                if (
                    e.PackedEntity == null || 
                    !e.PackedEntity.Value.Unpack(world, out var entityTest) ||
                    entityTest != entity
                ) {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            converter.Convert(gameObject, entity);

            return true;
        }

        public bool Convert<TS>(GameObject gameObject, out int entity) where TS : struct
        {
            var converter = Get<TS>();

            if (converter == null)
            {
                entity = -1;
                return false;
            }

            if (gameObject.TryGetComponent<EcsEntity>(out var e))
            {
                if (e.PackedEntity == null || !e.PackedEntity.Value.Unpack(world, out entity))
                {
                    entity = world.NewEntity();
                    e.PackedEntity = world.PackEntity(entity);
                }
            }
            else
            {
                entity = world.NewEntity();
                var ecsEntity = gameObject.AddComponent<EcsEntity>();
                ecsEntity.PackedEntity = world.PackEntity(entity);
            }

            converter.Convert(gameObject, entity);

            return true;
        }       
        
        public bool Convert<TS>(GameObject gameObject, int entity) where TS : struct
        {
            var converter = Get<TS>();

            if (converter == null)
            {
                return false;
            }

            if (gameObject.TryGetComponent<EcsEntity>(out var e))
            {
                if (e.PackedEntity == null || !e.PackedEntity.Value.Unpack(world, out entity))
                {
                    e.PackedEntity = world.PackEntity(entity);
                }
            }
            else
            {
                var ecsEntity = gameObject.AddComponent<EcsEntity>();
                ecsEntity.PackedEntity = world.PackEntity(entity);
            }

            converter.Convert(gameObject, entity);

            return true;
        }

        public async void ResolveDependencies()
        {
            foreach (var (key, value) in dictionary)
            {
                await DI.Resolve(value);
            }
        }
    }
}