using System;
using Leopotam.EcsProto.QoL;
using td.features._common.interfaces;
using td.features.building.components;
using td.features.building.data;
using td.features.level;
using td.features.level.cells;
using td.utils;
using td.utils.di;
using td.utils.ecs;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.building
{
    public class Building_Service
    {
        [DI] private Building_Aspect aspect;
        [DI] private Buildings_Config_SO buildingsConfigSO;
        [DI] private Level_State levelState;

        private Building_Config[] buildingConfigs;
        
        public bool HasBuilding(int entity) => aspect.buildingPool.Has(entity);
        public ref Building GetBuilding(int entity) => ref aspect.buildingPool.GetOrAdd(entity);

        public ref Building GetBuilding(ProtoPackedEntityWithWorld entity, out int buildEntity)
        {
            var check = entity.Unpack(out var w, out buildEntity);
#if UNITY_EDITOR
            if (!check)
            {
                throw new Exception($"Building component not found");
            }
#endif
            return ref GetBuilding(buildEntity);
        }

        public ref Building Init(int entity, string buildingId, int2 coords, IInputEventsHandler inputHandler)
        {
            ref var config = ref GetConfig(buildingId);
            ref var building = ref GetBuilding(entity);

            var count = GetCount(buildingId);
            
            building.id = config.id;
            building.price = CalcPrice(ref config, count);
            building.extraFeaturePriceMultiplier = CalcExtraFeaturePriceMultiplier(ref config);
            building.buildTimeRemaining = 0f;
            building.extraFeatureTimeRemaining = 0f;
            building.coords = coords;
            
            if (levelState.HasCell(coords, CellTypes.CanBuild)) {
                // ToDo
                ref var cell = ref levelState.GetCell(coords, CellTypes.CanBuild);
                cell.packedBuildingEntity = aspect.World().PackEntityWithWorld(entity);
                cell.buildingId = Constants.Buildings.ShardTower;
                cell.inputEventsHandlers.Add(inputHandler); // todo ???
            }
#if UNITY_EDITOR
            else
            {
                throw new Exception($"Cell {coords} is not type of CanBuild");
            }
#endif


            return ref building;
        }

        public ref Building_Config GetConfig(string buildingId)
        {
            if (buildingConfigs == null) buildingConfigs = ServiceContainer.Get<Building_Config[]>();
            
            for (var idx = 0; idx < buildingConfigs.Length; idx++)
            {
                if (buildingConfigs[idx].id == buildingId) return ref buildingConfigs[idx];
            }

            throw new Exception($"Building with id '{buildingId}' not found");
        }

        public int GetCount(string buildingId)
        {
            var count = 0;
            foreach (var entity in aspect.it)
            {
                ref var building = ref GetBuilding(entity);
                if (building.id == buildingId) count++;
            }
            return count; 
        }

        public Sprite GetIcon(string buildingId) => buildingsConfigSO.GetIcon(buildingId);

        
        public uint CalcPrice(string buildingId)
        {
            ref var config = ref GetConfig(buildingId);
            return CalcPrice(ref config);
        }
        public uint CalcPrice(ref Building_Config config, int count = -1)
        {
            var c = count > -1 ? count : GetCount(config.id);
            return config.price * (c > 0 ? (uint)(c * config.priceIncrease) : 1);
        }  
        
        
        public uint CalcBuildTime(string buildingId)
        {
            ref var config = ref GetConfig(buildingId);
            return CalcPrice(ref config);
        }
        public uint CalcBuildingTime(ref Building_Config config, int count = -1)
        {
            var c = count > -1 ? count : GetCount(config.id);
            return config.buildTime * (c > 0 ? (uint)(c * config.priceIncrease) : 1);
        }
        
        
        public float CalcExtraFeaturePriceMultiplier(string buildingId)
        {
            ref var config = ref GetConfig(buildingId);
            return CalcExtraFeaturePriceMultiplier(ref config);
        }
        public float CalcExtraFeaturePriceMultiplier(ref Building_Config config, int count = -1)
        {
            var c = count > -1 ? count : GetCount(config.id);
            return config.extraFeaturePrice * (c > 0 ? c * config.extraFeaturePriceIncrease : 1f);
        }

        
        public uint CalcExtraFeaturePrice(string buildingId, uint basePrice)
        {
            ref var config = ref GetConfig(buildingId);
            return CalcExtraFeaturePrice(ref config, basePrice);
        }
        public uint CalcExtraFeaturePrice(ref Building_Config config, uint basePrice) =>
            (uint)(basePrice * CalcExtraFeaturePriceMultiplier(ref config));
        
        
        public float CalcExtraFeatureTimeMultiplier(string buildingId)
        {
            ref var config = ref GetConfig(buildingId);
            return CalcExtraFeatureTimeMultiplier(ref config);
        }
        public float CalcExtraFeatureTimeMultiplier(ref Building_Config config, int count = -1)
        {
            var c = count > -1 ? count : GetCount(config.id);
            return config.extraFeatureTime * (c > 0 ? (uint)(c * config.extraFeatureTimeIncrease) : 1);
        }
        
        
        public uint CalcExtraFeatureTime(string buildingId, uint basePrice)
        {
            ref var config = ref GetConfig(buildingId);
            return CalcExtraFeatureTime(ref config, basePrice);
        }
        public uint CalcExtraFeatureTime(ref Building_Config config, uint basePrice) =>
            (uint)(basePrice * CalcExtraFeatureTimeMultiplier(ref config));
    }
}