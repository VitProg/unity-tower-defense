using System.Collections;
using System.Collections.Generic;
using td.common;
using td.monoBehaviours;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.monoBehaviours
{
    public class ShowDebugInfo : MonoBehaviour
    {
        public void OnCheckBoxChanged()
        {
            DebugUtils.debugInfoVisible = !DebugUtils.debugInfoVisible;
        }
    }
}
