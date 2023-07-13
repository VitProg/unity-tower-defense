using td.common;
using td.monoBehaviours;
using td.utils.ecs;
using UnityEngine;

namespace td.utils
{
    public static class DebugUtils
    {
        private static bool _debugInfoVisible = false;
        
        public static bool debugInfoVisible
        {
            get => _debugInfoVisible;
            set
            {
                if (!DI.IsReady) return;
                
                if (_debugInfoVisible == value) return;
                _debugInfoVisible = value;

                var shared = DI.GetShared<SharedData>()!;
                
                // @see https://discussions.unity.com/t/edit-camera-culling-mask/55812/3
                // Toggle
                shared.mainCamera.cullingMask ^= 1 << LayerMask.NameToLayer("Debug1");
                shared.mainCamera.cullingMask ^= 1 << LayerMask.NameToLayer("Debug2");

                /*if (_debugInfoVisible)
                {
                    //Show
                    shared.mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Debug1");
                    shared.mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Debug2");
                }
                else
                {
                    //Hide
                    shared.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Debug1"));
                    shared.mainCamera.cullingMask &= ~(2 << LayerMask.NameToLayer("Debug2"));
                }*/
            }
        }
    }
}