using Leopotam.EcsLite;
using td.utils;
using td.utils.ecs;
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
            var world = DI.GetWorld();
            
            /**/
            if (world != null && info.ecsEntity.TryGetEntity(out var entity))
            {
                var components = new object[] { };
                world.GetComponents(entity, ref components);

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
