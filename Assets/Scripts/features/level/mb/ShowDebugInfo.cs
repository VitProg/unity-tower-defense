using td.utils.di;
using UnityEngine;

namespace td.features.level.mb
{
    public class ShowDebugInfo : MonoBehaviour
    {
        public void OnCheckBoxChanged()
        {
            var lms = ServiceContainer.Get<Level_Map_Service>();
            lms.DebugInfoVisible = !lms.DebugInfoVisible;
        }
    }
}
