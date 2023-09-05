using Leopotam.EcsProto.QoL;
using td.features.camera;
using UnityEngine;

namespace td.features.level
{
    public class Level_Map_Service
    {
        [DI] private Camera_Service cameraService;

        private bool debugInfoVisible = false;
        public bool DebugInfoVisible
        {
            get => debugInfoVisible;
            set
            {
                if (debugInfoVisible == value) return;
                debugInfoVisible = value;

                cameraService.GetMainCamera().cullingMask ^= 1 << LayerMask.NameToLayer("Debug1");
                cameraService.GetMainCamera().cullingMask ^= 1 << LayerMask.NameToLayer("Debug2");
            }
        }
    }

    public enum CanDropShardOnMapType
    {
        FalseDropToFloor = -4,
        FalseCombineInTower = -3,
        // FalseCombineCost = -2,
        FalseInsertInTower = -1,
        False = 0,
        InsertInTower = 1,
        // Combine = 2,
        CombineInTower = 3,
        DropToFloor = 4,
    }
}