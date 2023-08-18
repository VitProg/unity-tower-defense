#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state;
using td.utils;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace td.features.building.buildingShop.state
{

    [Serializable]
    public class BuildingShop_StateEx : IStateExtension
    {
        [DI] private readonly EventBus events;
        private static Type evType = typeof(Event_BuildingShop_StateChanged);
        private Event_BuildingShop_StateChanged ev;

        #region Private Fields
        private BuildingShop_Item[] items = new BuildingShop_Item[6];
        private uint count = 0;
        private bool visible;
        private int2 cellCoords;
        #endregion
        
        #region Getters
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref BuildingShop_Item[] GetBuildings() => ref items;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public uint GetCount() => count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasBuilding(string buildingId)
        {
            for (var idx = 0; idx < count; idx++)
            {
                if (items[idx].id == buildingId) return true;
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool GetVisible() => visible;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref int2 GetCellCoords() => ref cellCoords;
        #endregion
        
        #region Setters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearBuildings()
        {
            if (count == 0) return;
            count = 0;
            for (var idx = 0; idx < items.Length; idx++)
            {
                items[idx] = default;
            }
            ev.items = true;
            ev.changedItem = -1;
        }
        
        public void SetBuilding(ref BuildingShop_Item item)
        {
            for (var idx = 0; idx < count; idx++)
            {
                if (items[idx].id == item.id)
                {
                    items[idx].price = item.price;
                    ev.items = true;
                    ev.changedItem = idx;
                    return;
                }
            }

#if UNITY_EDITOR
            if (count + 1 >= items.Length) throw new Exception("Buildings limit reached");
#endif
            
            items[count] = item;
            
            ev.items = true;
            ev.changedItem = (int)count;
                
            count++;
        }
        
        public void SetVisible(bool value)
        {
            if (visible == value) return;
            visible = value;
            ev.visible = true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCellCoords(int x, int y)
        {
            if (x == cellCoords.x && y == cellCoords.y) return;
            cellCoords.x = x;
            cellCoords.y = y;
            ev.position = true;
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetEventType() => evType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Refresh() => ev.All();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            visible = false;
            cellCoords.x = 0;
            cellCoords.y = 0;
            ClearBuildings();
            ev.All();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendChanges()
        {
            if (!ev.IsEmpty())
            {
                events.unique.GetOrAdd<Event_BuildingShop_StateChanged>() = ev;
            }
            ev = default;
        }

#if UNITY_EDITOR
        public void DrawStateProperties(VisualElement root)
        {
            EditorUtils.DrawTitle("Building Menu", true);
            EditorGUI.indentLevel++;
            EditorUtils.DrawProperty("Visible", visible);
            EditorUtils.DrawProperty("Cell Coords", cellCoords);
            
            if (EditorUtils.FoldoutBegin("building_shop_items", $"Items ({count})"))
            {
                EditorGUI.indentLevel++;
                for (var idx = 0; idx < count; idx++)
                {
                    EditorGUILayout.LabelField($"[{idx}]");
                    EditorGUI.indentLevel++;
                    EditorUtils.DrawProperty("Building ID" , items[idx].id);
                    EditorUtils.DrawProperty("Price" , items[idx].price);
                    EditorUtils.DrawProperty("Build Time" , items[idx].buildTime);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.indentLevel--;
        }
#endif
    }
}