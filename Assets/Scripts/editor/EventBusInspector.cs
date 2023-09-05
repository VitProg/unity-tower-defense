using System;
using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.Unity;
using td.features.eventBus;
using td.features.shard;
using td.features.state;
using td.utils;
using td.utils.di;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Font = UnityEngine.Font;
using FontStyle = UnityEngine.FontStyle;

namespace td.editor
{
    public class EventBusInspector : EditorWindow
    {
        private bool initialized = false;
        private State state;
        private Shard_Calculator calc;
        private Vector2 scrollPosition;
        private EventBus events;

        [MenuItem("TD/EventBus Log", false, -2000)]
        public static void ShowWindow()
        {
            EditorWindow wnd = GetWindow<EventBusInspector>();
            wnd.titleContent = new GUIContent("EventBus Log");
        }

        private Rect upperPanel;
        private Rect lowerPanel;
        private Rect resizer;
        private Rect menuBar;

        private float sizeRatio = 0.5f;
        private bool isResizing;
 
        private float resizerHeight = 5f; 
        private float menuBarHeight = 20f;
        private float lineHeight = 20f;
     
        private bool hideStateChanges = true;
        private bool clearOnPlay = false;
        private bool errorPause = false;
        private bool showLog = false;
        private bool showWarnings = false;
        private bool showErrors = false;
 
        private Vector2 upperPanelScroll;
        private Vector2 lowerPanelScroll;

        private GUIStyle panelStyle;
        private GUIStyle resizerStyle;
        private GUIStyle boxStyle;
        private GUIStyle lableStyle;
        private GUIStyle boldLableStyle;
 
        private Texture2D boxBgOdd;
        private Texture2D boxBgEven;
        private Texture2D boxBgSelected;

        private int selectedIdx = -1;

        private const int Max = 1000;
        private Slice<LogItem> logs = new(Max);
        private List<int> deleteFromLogs = new(Max / 2);

        private readonly int monoFontSize = 12;
        private Font monoFont = null;

        private readonly Color defaultColor = new Color(0.8f, 0.8f, 0.8f);

        private void OnEnable()
        {
            monoFont = Font.CreateDynamicFontFromOSFont(new []{"Consolas", "SF Mono", "DejaVu Sans Mono", "Roboto Mono", "Courier New"}, monoFontSize);
            lineHeight = monoFont.lineHeight + 2;
            
            panelStyle = new GUIStyle();
            panelStyle.padding = new RectOffset(5, 5, 10, 10);
            panelStyle.font = monoFont;

            lableStyle = new GUIStyle();
            lableStyle.font = monoFont;
            lableStyle.normal.textColor = defaultColor;

            boldLableStyle = new GUIStyle();
            boldLableStyle.fontStyle = FontStyle.Bold;
            boldLableStyle.font = monoFont;
            boldLableStyle.normal.textColor = defaultColor;
            
            resizerStyle = new GUIStyle();
            resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

            boxStyle = new GUIStyle();
            boxStyle.font = monoFont;
            boxStyle.padding.top = 3;
            boxStyle.normal.textColor = defaultColor;
        
            boxBgOdd = EditorGUIUtility.Load("builtin skins/darkskin/images/cn entrybackodd.png") as Texture2D;
            boxBgEven = EditorGUIUtility.Load("builtin skins/darkskin/images/cnentrybackeven.png") as Texture2D;
            boxBgSelected = EditorGUIUtility.Load("builtin skins/darkskin/images/menuitemhover.png") as Texture2D;
        }

        private void OnGUI()
        {
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

                events.unique.GlobalListenTo(OnEvent);
                events.global.GlobalListenTo(OnEvent);

                initialized = true;
                
                logs.Clear();
            }

            Draw();
        }


        private void OnEvent(ref object evData)
        {
            var type = evData.GetType();
            var clearType = EditorExtensions.GetCleanTypeName(type);
            
            if (clearType == "Event_StageSomeChanged") return;
            if (hideStateChanges && IsStateEvent(clearType)) return;

            logs.Add(default);
            ref var l = ref logs.Get(logs.Len() - 1);
            l.Data = evData;
            l.ClearName = clearType;
            l.Module = type.Assembly.GetName().Name.Split('.')[^1];
            var now = DateTime.Now;
            l.Time = $"[{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D4}]";
            
            Repaint();
        }


        
        private bool stylesInitialized;

        private void Draw()
        {
            DrawMenuBar();
            DrawUpperPanel();
            DrawLowerPanel();
            DrawResizer();

            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }
        
        private void DrawMenuBar()
        {
            menuBar = new Rect(0, 0, position.width, menuBarHeight);
 
            GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Clear"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                logs.Clear();
                GUI.changed = true;
            }
 
            GUILayout.Space(5);
 
            var t = GUILayout.Toggle(hideStateChanges, new GUIContent("Hide State Changes"), EditorStyles.toolbarButton, GUILayout.Width(140));
            if (hideStateChanges != t)
            {
                hideStateChanges = t;

                if (hideStateChanges)
                {
                    var len = logs.Len();
                    
                    deleteFromLogs.Clear();
                    for (var idx = 0; idx < len; idx++)
                    {
                        ref var item = ref logs.Get(idx);
                        if (IsStateEvent(item.ClearName))
                        {
                            deleteFromLogs.Add(idx);
                        }
                    }
                    for (var idx = deleteFromLogs.Count - 1; idx >= 0; idx--)
                    {
                        logs.RemoveAt(deleteFromLogs[idx]);
                    }
                    deleteFromLogs.Clear();
                }
                
                GUI.changed = true;
            }
 
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        
        private void DrawUpperPanel()
        {
            upperPanel = new Rect(0, menuBarHeight, position.width, (position.height * sizeRatio) - menuBarHeight);

            GUILayout.BeginArea(upperPanel, panelStyle);
            upperPanelScroll = GUILayout.BeginScrollView(upperPanelScroll);

            var len = logs.Len();
            var odd = false;
            for (var idx = len - 1; idx >= 0; idx--)
            {
                ref var item = ref logs.Get(idx);
                var isSelected = selectedIdx == idx;

                if (!hideStateChanges || (hideStateChanges && !IsStateEvent(item.ClearName)))
                {
                    if (DrawBox(ref item, odd, isSelected))
                    {
                        selectedIdx = idx;
                        GUI.changed = true;
                    }
                    odd = !odd;
                }
                else
                {
                    if (isSelected)
                    {
                        selectedIdx = -1;
                        GUI.changed = true;
                    }
                }
            }
 
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        
        private void DrawLowerPanel()
        {
            lowerPanel = new Rect(0, (position.height * sizeRatio) + resizerHeight, position.width, (position.height * (1 - sizeRatio)) - resizerHeight);

            GUILayout.BeginArea(lowerPanel, panelStyle);
            lowerPanelScroll = GUILayout.BeginScrollView(lowerPanelScroll);
 
            if (selectedIdx >= 0 && selectedIdx < logs.Len())
            {
                ref var item = ref logs.Get(selectedIdx);
                var evType = item.Data.GetType();

                EditorGUILayout.LabelField("Selected Event", boldLableStyle);
                EditorUtils.DrawLine(1, 2, 2, Color.grey);
                EditorGUILayout.LabelField(item.Time, lableStyle);
                EditorGUILayout.LabelField(item.Module, lableStyle);
                EditorGUILayout.LabelField(item.ClearName, lableStyle);
                EditorUtils.DrawLine(1, 2, 2, Color.grey);
                foreach (var field in evType.GetFields())
                {
                    EditorUtils.LabelField(field.Name, field.GetValue(item.Data).ToString(), lableStyle);
                }
            }
 
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawResizer()
        {
            resizer = new Rect(0, (position.height * sizeRatio) - resizerHeight, position.width, resizerHeight * 2);

            GUILayout.BeginArea(new Rect(resizer.position + (Vector2.up * resizerHeight), new Vector2(position.width, 2)), resizerStyle);
            GUILayout.EndArea();

            EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeVertical);
        }
        
        private bool DrawBox(ref LogItem item, bool isOdd, bool isSelected)
        {
            if (isSelected)
            {
                boxStyle.normal.background = boxBgSelected;
            }
            else
            {
                boxStyle.normal.background = isOdd ? boxBgOdd : boxBgEven;
            }

            return GUILayout.Button(
                item.Time + "  " + item.Module + " - " + item.ClearName,
                boxStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(lineHeight)
            );
        }
        
        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && resizer.Contains(e.mousePosition))
                    {
                        isResizing = true;
                    }
                    break;

                case EventType.MouseUp:
                    isResizing = false;
                    break;
                
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.DownArrow) Prev();
                    if (e.keyCode == KeyCode.UpArrow) Next();
                    if (e.keyCode == KeyCode.PageDown) for (var i = 0; i < 10; i++) Prev();
                    if (e.keyCode == KeyCode.PageUp) for (var i = 0; i < 10; i++) Next();
                    break;
            }

            Resize(e);
        }

        private void Next()
        {
            var s = selectedIdx;
            var len = logs.Len();
            s = Math.Min(len - 1, s + 1);
            if (hideStateChanges)
            {
                while (IsStateEvent(logs.Get(s).ClearName) && s != selectedIdx && s < len)
                {
                    var s2 = Math.Min(len, s + 1);
                    if (s != s2) s = s2;
                    else break;
                }
            }

            if (s != selectedIdx)
            {
                selectedIdx = s;
                GUI.changed = true;
            }
        }     
        
        private void Prev()
        {
            var s = selectedIdx;
            s = Math.Max(0, s - 1);
            if (hideStateChanges)
            {
                while (IsStateEvent(logs.Get(s).ClearName) && s != selectedIdx && s > 0)
                {
                    var s2 = Math.Max(0, s - 1);
                    if (s != s2) s = s2;
                    else break;
                }
            }

            if (s != selectedIdx)
            {
                selectedIdx = s;
                GUI.changed = true;
            }
        }

        private void Resize(Event e)
        {
            if (isResizing)
            {
                sizeRatio = e.mousePosition.y / position.height;
                Repaint();
            }
        }
        
        private void OnDestroy()
        {
            events.unique.RemoveGlobalListener(OnEvent);
            events.global.RemoveGlobalListener(OnEvent);
        }
        
        private bool IsStateEvent(string clearName) => clearName == "Event_StateChanged" || (clearName.StartsWith("Event_") && clearName.EndsWith("_StateChanged"));
    }
    
    internal struct LogItem
    {
        public object Data;
        public string ClearName;
        public string Time;
        public string Module;
    }
}