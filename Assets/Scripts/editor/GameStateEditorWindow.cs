using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.shard;
using td.features.state;
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

                events.unique.ListenTo<Event_StateChanged>(OnStateChanged);

                initialized = true;
            }

            DrawStateProperties();
        }

        private void OnDestroy()
        {
            events?.unique.RemoveListener<Event_StateChanged>(OnStateChanged);
        }

        private void OnStateChanged(ref Event_StateChanged item)
        {
            Repaint();
        }

        private void DrawStateProperties()
        {
            rootVisualElement.Clear();
            
            EditorUtils.DrawTitle("Game State");
            EditorGUI.indentLevel++;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            // EditorGUI.BeginDisabledGroup(true);
            
            state.DrawStateProperties(rootVisualElement);

            // DrawProperty("Max Lives", state.MaxLives);
            // DrawProperty("Lives", state.Lives);
            // DrawProperty("Level Number", state.LevelNumber);
            // DrawProperty("Energy", state.Energy);
            // DrawProperty("Next Wave Countdown", state.NextWaveCountdown);
            // DrawProperty("Wave Number", state.WaveNumber);
            // DrawProperty("Wave Count", state.WaveCount);
            // DrawProperty("Active Spawn Count", state.ActiveSpawnCount);
            // DrawProperty("Enemies Count", state.EnemiesCount);
            // DrawProperty("Game Speed", state.GameSpeed);
            //
            // state.Ex<>()
            //
            // DrawTitle("Info Panel State", true);
            // EditorGUI.indentLevel++;
            // DrawProperty("Visible", state.InfoPanel.Visible);
            // DrawProperty("Cost", state.InfoPanel.Cost);
            // DrawProperty("Cost Title", state.InfoPanel.CostTitle);
            // DrawProperty("Before Text", state.InfoPanel.Before);
            // DrawProperty("After Text", state.InfoPanel.After);
            // if (state.InfoPanel.Shard.HasValue)
            // {
            //     DrawTitle("Shard:");
            //     EditorGUI.indentLevel++;
            //     DrawProperty(state.InfoPanel.Shard);
            //     EditorGUI.indentLevel--;
            // }
            //
            // if (state.InfoPanel.Enemy.HasValue)
            // {
            //     DrawTitle("Enemy:");
            //     EditorGUI.indentLevel++;
            //     DrawProperty(state.InfoPanel.Enemy);
            //     EditorGUI.indentLevel--;
            // }
            //
            // EditorGUI.indentLevel--;
            //
            //
            // DrawTitle("Cost Popup State", true);
            // EditorGUI.indentLevel++;
            // DrawProperty("Visible", state.CostPopup.Visible);
            // DrawProperty("Title", state.CostPopup.Title);
            // DrawProperty("Cost", state.CostPopup.Cost);
            // DrawProperty("IsFine", state.CostPopup.IsFine);
            // EditorGUI.indentLevel--;
            //
            // DrawTitle("Shard Store State", true);
            // EditorGUI.indentLevel++;
            // DrawProperty("Visible", state.ShardStore.Visible);
            // DrawProperty("Position X", state.ShardStore.X);
            // if (FoldoutBegin("shard_store_items", $"Items ({state.ShardStore.Items.Count})"))
            // {
            //     foreach (var item in state.ShardStore.Items)
            //     {
            //         DrawProperty("cost", item.cost);
            //         DrawProperty("type", item.shardType.ToString());
            //     }
            // }
            // EditorGUI.indentLevel--;
            //
            // DrawTitle("Shard Collection State", true);
            // EditorGUI.indentLevel++;
            // DrawProperty("MaxItems", state.ShardCollection.MaxItems);
            // if (FoldoutBegin("shard_collection_items", $"Items ({state.ShardCollection.Items.Count})"))
            // {
            //     foreach (var item in state.ShardCollection.Items)
            //     {
            //         DrawProperty(item);
            //     }
            // }
            // EditorGUI.indentLevel--;
            //
            // if (state.ShardCollection.HoveredItem && state.ShardCollection.HoveredItem.hasShard)
            // {
            //     Shard? shard = state.ShardCollection.HoveredItem.GetShard();
            //     DrawTitle("Hovered Shard");
            //     EditorGUI.indentLevel++;
            //     DrawProperty(shard);
            //     EditorGUI.indentLevel--;
            // }

            // EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;
        }

        
    }
}