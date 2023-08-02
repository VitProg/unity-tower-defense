using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leopotam.EcsLite;
using td.features.enemy.components;
using td.features.shard;
using td.features.shard.components;
using td.features.state;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace td.editor
{
    public class GameStateEditorWindow : EditorWindow
    {
        private bool initialized = false;
        private IServiceContainer c;
        private IState state;
        private ShardCalculator calc;
        private Vector2 scrollPosition;
        private IEventBus events;

        [MenuItem("TD/Game State Window", false, -2000)]
        public static void ShowWindow()
        {
            EditorWindow wnd = GetWindow<GameStateEditorWindow>();
            wnd.titleContent = new GUIContent("Game State");
        }

        private void OnGUI()
        {
            boldStyle = new GUIStyle()
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
                c = ServiceContainer.GetCurrentContainer();

                if (c == null)
                {
                    rootVisualElement.Add(new Label("ServiceContainer not found!"));
                    return;
                }

                state = c.Get<IState>();

                if (state == null)
                {
                    rootVisualElement.Add(new Label("State not found!"));
                    return;
                }

                calc = c.Get<ShardCalculator>();

                if (calc == null)
                {
                    rootVisualElement.Add(new Label("ShardCalculator not found!"));
                    return;
                }

                events = c.Get<IEventBus>();

                if (events == null)
                {
                    rootVisualElement.Add(new Label("IEventBus not found!"));
                    return;
                }

                events.Unique.ListenTo<Event_StateChanged>(OnStateChanged);

                initialized = true;
            }

            var t = DrawStateProperties();
        }

        private void OnDestroy()
        {
            events?.Unique.RemoveListener<Event_StateChanged>(OnStateChanged);
        }

        private void OnStateChanged(ref Event_StateChanged item)
        {
            Repaint();
        }

        private async Task DrawStateProperties()
        {
            rootVisualElement.Clear();
            
            DrawTitle("Game State");
            EditorGUI.indentLevel++;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            // EditorGUI.BeginDisabledGroup(true);

            DrawProperty("Max Lives", state.MaxLives);
            DrawProperty("Lives", state.Lives);
            DrawProperty("Level Number", state.LevelNumber);
            DrawProperty("Energy", state.Energy);
            DrawProperty("Next Wave Countdown", state.NextWaveCountdown);
            DrawProperty("Wave Number", state.WaveNumber);
            DrawProperty("Wave Count", state.WaveCount);
            DrawProperty("Active Spawn Count", state.ActiveSpawnCount);
            DrawProperty("Enemies Count", state.EnemiesCount);
            DrawProperty("Game Speed", state.GameSpeed);

            DrawTitle("Info Panel State", true);
            EditorGUI.indentLevel++;
            DrawProperty("Visible", state.InfoPanel.Visible);
            DrawProperty("Cost", state.InfoPanel.Cost);
            DrawProperty("Cost Title", state.InfoPanel.CostTitle);
            DrawProperty("Before Text", state.InfoPanel.Before);
            DrawProperty("After Text", state.InfoPanel.After);
            if (state.InfoPanel.Shard.HasValue)
            {
                DrawTitle("Shard:");
                EditorGUI.indentLevel++;
                DrawProperty(state.InfoPanel.Shard);
                EditorGUI.indentLevel--;
            }

            if (state.InfoPanel.Enemy.HasValue)
            {
                DrawTitle("Enemy:");
                EditorGUI.indentLevel++;
                DrawProperty(state.InfoPanel.Enemy);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;


            DrawTitle("Cost Popup State", true);
            EditorGUI.indentLevel++;
            DrawProperty("Visible", state.CostPopup.Visible);
            DrawProperty("Title", state.CostPopup.Title);
            DrawProperty("Cost", state.CostPopup.Cost);
            DrawProperty("IsFine", state.CostPopup.IsFine);
            EditorGUI.indentLevel--;

            DrawTitle("Shard Store State", true);
            EditorGUI.indentLevel++;
            DrawProperty("Visible", state.ShardStore.Visible);
            DrawProperty("Position X", state.ShardStore.X);
            if (FoldoutBegin("shard_store_items", $"Items ({state.ShardStore.Items.Count})"))
            {
                foreach (var item in state.ShardStore.Items)
                {
                    DrawProperty("cost", item.cost);
                    DrawProperty("type", item.shardType.ToString());
                }
            }
            EditorGUI.indentLevel--;

            DrawTitle("Shard Collection State", true);
            EditorGUI.indentLevel++;
            DrawProperty("MaxItems", state.ShardCollection.MaxItems);
            if (FoldoutBegin("shard_collection_items", $"Items ({state.ShardCollection.Items.Count})"))
            {
                foreach (var item in state.ShardCollection.Items)
                {
                    DrawProperty(item);
                }
            }
            EditorGUI.indentLevel--;

            if (state.ShardCollection.HoveredItem && state.ShardCollection.HoveredItem.hasShard)
            {
                Shard? shard = state.ShardCollection.HoveredItem.GetShard();
                DrawTitle("Hovered Shard");
                EditorGUI.indentLevel++;
                DrawProperty(shard);
                EditorGUI.indentLevel--;
            }

            // EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;
        }

        private void DrawTitle(string sTitle, bool sep = false)
        {
            if (sep)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();
            }

            EditorGUILayout.LabelField(sTitle);//, boldStyle);
        }

        private void DrawProperty(string sNname, bool value)
        {
            EditorGUILayout.TextField(sNname, value ? "True" : "False");
        }

        private void DrawProperty(string sNname, float value)
        {
            EditorGUILayout.FloatField(sNname, value);
        }

        private void DrawProperty(string sNname, ushort value)
        {
            EditorGUILayout.IntField(sNname, value);
        }

        private void DrawProperty(string sNname, uint value)
        {
            EditorGUILayout.LongField(sNname, value);
        }

        private void DrawProperty(string sNname, int value)
        {
            EditorGUILayout.IntField(sNname, value);
        }

        private void DrawProperty(string sNname, string value)
        {
            EditorGUILayout.LabelField(sNname, value);
        }

        private void DrawProperty(Enemy? enemy)
        {
            if (!enemy.HasValue) return;
            DrawTitle("Enemy");
            EditorGUI.indentLevel++;
            //todo
            EditorGUI.indentLevel--;
        }

        private Dictionary<string, bool> foldouts = new();
        private GUIStyle boldStyle;
        private bool drawStatePropertiesScheduled;

        private bool FoldoutBegin(string key, string title, string titleHidden = null)
        {
            EditorGUI.indentLevel++;
            var value = foldouts.TryGetValue(key, out var f) && f;
            foldouts[key] = value;
            foldouts[key] = EditorGUILayout.Foldout(value,
                value ? title : (string.IsNullOrEmpty(titleHidden) ? title : titleHidden));
            return value;
        }

        private void FoldoutEnd()
        {
            EditorGUI.indentLevel--;
            // EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawProperty(Shard? shard)
        {
            if (!shard.HasValue) return;

            if (FoldoutBegin(shard.ToString(), "Shard", $"Shard {shard}"))
            {
                // DrawTitle("Shard");
                // EditorGUI.indentLevel++;
                DrawProperty("Level", calc.GetShardLevel(shard.Value.Quantity));
                DrawProperty("Quantity", shard.Value.Quantity);
                EditorGUILayout.Space();
                DrawProperty("red", shard.Value.red);
                DrawProperty("green", shard.Value.green);
                DrawProperty("blue", shard.Value.blue);
                DrawProperty("aquamarine", shard.Value.aquamarine);
                DrawProperty("yellow", shard.Value.yellow);
                DrawProperty("orange", shard.Value.orange);
                DrawProperty("pink", shard.Value.pink);
                DrawProperty("violet", shard.Value.violet);
                EditorGUILayout.Space();
                DrawProperty("Cost Buy", shard.Value.cost);
                DrawProperty("Cost Insert", shard.Value.costInsert);
                DrawProperty("Cost Remove", shard.Value.costRemove);
                DrawProperty("Cost Combine", shard.Value.costCombine);
                DrawProperty("Cost Drop", shard.Value.costDrop);
            }
            FoldoutEnd();

            // EditorGUI.indentLevel--;
        }

/*        private void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            var lastPropPath = string.Empty;
            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();
                    
                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) continue;
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }
*/
    }
}