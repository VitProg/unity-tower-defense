using Leopotam.EcsLite;
using td.utils;
using UnityEditor;

namespace td.monoBehaviours
{
#if UNITY_EDITOR
    [CustomEditor(typeof(EcsComponentsInfo))]
    public class EcsComponentsInfoEditor : Editor
    {
        private string id;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            
            var info = (EcsComponentsInfo)target;
            
            /**/
            if (info.World != null && info.PackedEntity.Unpack(info.World, out var entity))
            {
                var components = new object[] { };
                info.World.GetComponents(entity, ref components);

                foreach (var component in components)
                {
                    EditorUtils.RenderAllPropertiesOfObject(ref id, component, "*");
                }
            }

            EditorGUI.EndChangeCheck();
        }
    }
#endif
}
