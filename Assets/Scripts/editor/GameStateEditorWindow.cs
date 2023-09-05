using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.shard;
using td.features.state;
using td.features.state.bus;
using td.utils;
using td.utils.di;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace td.editor
{
    public class GameStateEditorWindow : EditorWindow
    {
        private bool initialized = false;
        private State state;
        private Shard_Calculator calc;
        private Vector2 scrollPosition;
        private EventBus events;
        private GUIStyle wrapperStyle;

        [MenuItem("TD/Game State Window", false, -2000)]
        public static void ShowWindow()
        {
            EditorWindow wnd = GetWindow<GameStateEditorWindow>();
            wnd.titleContent = new GUIContent("Game State");
        }

        private void OnGUI()
        {
            EditorUtils.boldStyle = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
            };
            
            rootVisualElement.Clear();
            if (!Application.isPlaying)
            {
                rootVisualElement.Add(new Label("Work only in play mode!"));
                return;
            }

            if (!initialized || state == null)
            {
                state = ServiceContainer.Get<State>();

                if (state == null)
                {
                    rootVisualElement.Add(new Label("State not found!"));
                    return;
                }

                calc = ServiceContainer.Get<Shard_Calculator>();

                if (calc == null)
                {
                    rootVisualElement.Add(new Label("ShardCalculator not found!"));
                    return;
                }

                events = ServiceContainer.Get<EventBus>();

                if (events == null)
                {
                    rootVisualElement.Add(new Label("IEventBus not found!"));
                    return;
                }

                events.unique.ListenTo<Event_StageSomeChanged>(OnStateChanged);

                initialized = true;
            }

            DrawStateProperties();
        }

        private void OnDestroy()
        {
            events?.unique.RemoveListener<Event_StageSomeChanged>(OnStateChanged);
        }

        private void OnStateChanged(ref Event_StageSomeChanged item)
        {
            Repaint();
        }

        private void DrawStateProperties()
        {
            rootVisualElement.Clear();

            if (wrapperStyle == null)
            {
                wrapperStyle = new GUIStyle();
                wrapperStyle.padding = new RectOffset(5, 5, 8, 8);
            }

            var rect = rootVisualElement.contentRect;
            GUILayout.BeginArea(rect, wrapperStyle);
            
            EditorGUILayout.LabelField("Game State", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.BeginVertical();
            state.DrawStateProperties(rootVisualElement);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
            
            GUILayout.EndArea();
        }

        
    }
}