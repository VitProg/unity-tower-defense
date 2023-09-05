#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features.eventBus;
using td.features.state.interfaces;
using td.utils;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace td.features.enemy.enemyPath {
    public class EnemyPath_State : IStateExtension {
        [DI] private readonly EventBus events;
        private static readonly Type EvType = typeof(Event_EnemyPath_StateChanged);
        private Event_EnemyPath_StateChanged ev;

        #region Private Fields
        private readonly Slice<Slice<int2>> routes = new(8);
        #endregion
        
        #region Getters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCount() => routes.Len();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasRoute(int idx) => idx < routes.Len();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetRouteLength(int reouteIdx) => routes.Get(reouteIdx).Len();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RouteJasItem(int reouteIdx, int itemIdx) => itemIdx < routes.Get(reouteIdx).Len();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasRouteItem(int routeIdx, int itemIdx) => itemIdx < routes.Get(routeIdx).Len();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref int2 GetRouteItem(int routeIdx, int itemIdx) => ref routes.Get(routeIdx).Get(itemIdx);

        public bool RouteHaveCoord(int routeIdx, int x, int y) {
            ref var route = ref routes.Get(routeIdx);
            for (var idx = 0; idx < route.Len(); idx++) {
                ref var item = ref route.Get(idx);
                if (item.x == x && item.y == y) return true;
            }
            return false;
        }
        
        public int GetRouteItemIndexByCoord(int routeIdx, int x, int y) {
            ref var route = ref routes.Get(routeIdx);
            for (var idx = 0; idx < route.Len(); idx++) {
                ref var item = ref route.Get(idx);
                if (item.x == x && item.y == y) return idx;
            }
            return -1;
        }

        public int DeleteRoute(int routeIdx) {
            for (var idx = routeIdx; idx < routes.Len() - 1; idx++) {
                routes.Get(idx) = routes.Get(idx + 1);
            }
            routes.RemoveLast();
            return routes.Len();
        }

        public bool RoutesByFirstCoord(int x, int y, ref int[] findedRoutes, out int count) {
            count = 0;
            for (var idx = 0; idx < routes.Len(); idx++) {
                ref var route = ref routes.Get(idx);
                if (route.Len() > 0 && route.Get(0).x == x && route.Get(0).y == y && count + 1 < findedRoutes.Length) {
                    findedRoutes[count++] = idx;
                }
            }
            return count > 0;
        }
        #endregion
        
        #region Setters
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearAllRoutes() {
            for (var idx = 0; idx < routes.Len(); idx++) {
                routes.Get(idx).Clear();
            }
            routes.Clear();
            ev.routes = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRoute(int routeIdx) {
            routes.Get(routeIdx).Clear();
            ev.routes = true;
            ev.routeIdx = routeIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NewRoute() {
            routes.Add(new Slice<int2>(16));
            ev.routeIdx = routes.Len() - 1;
            ev.routes = true;
            return ev.routeIdx;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CopyRoute(int sourceRouteIdx) {
            ref var sourceRoute = ref routes.Get(sourceRouteIdx);
            var routeIdx = NewRoute();
            ref var route = ref routes.Get(routeIdx);
            var routeLength = sourceRoute.Len();
            for (var itemIdx = 0; itemIdx < routeLength; itemIdx++) {
                route.Add(sourceRoute.Get(itemIdx));
            }
            ev.routeIdx = routeIdx;
            ev.routes = true;
            return ev.routeIdx;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRouteItem(int routeIdx, int x, int y) {
            routes.Get(routeIdx).Add(new int2(x, y));
            ev.routeIdx = routeIdx;
            ev.routes = true;
        }

        public int ClearEmptyRoutes() {
            for (var idx = 0; idx < routes.Len(); idx++) {
                ref var route = ref routes.Get(idx);
                if (route.Len() < 3) {
                    if (idx + 1 < routes.Len()) {
                        routes.Get(idx) = routes.Get(idx + 1);
                    } else {
                        routes.RemoveLast();
                    }
                }
            }
            return routes.Len();
        }
#endregion
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetEventType() => EvType;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SendChanges() {
            if (ev.IsEmpty()) return false;
            events.unique.GetOrAdd<Event_EnemyPath_StateChanged>() = ev;
            ev = default;
            return true;
        }

        public void Clear() {
            ClearAllRoutes();
            ev.Clear();
        }

        public void Refresh() {
            ev.All();
        }

#if UNITY_EDITOR
        public string GetStateName() => "Enemy Path";

        public void DrawStateProperties(VisualElement root) {
            var routesLength = routes.Len();
            if (EditorUtils.FoldoutBegin("enemy_path__routes", $"Routes ({routesLength})")) {
                for (var routeIdx = 0; routeIdx < routesLength; routeIdx++) {
                    EditorGUI.indentLevel++;
                    ref var route = ref routes.Get(routeIdx);
                    var routeLength = route.Len();
                    if (routeLength == 0) {
                        EditorGUILayout.LabelField($"Route[{routeIdx}] (empty)");
                    } else if (routeLength == 1) {
                        EditorGUILayout.LabelField($"Route[{routeIdx}] (1) {route.Get(0)}");
                    } else {
                        EditorGUILayout.LabelField($"Route[{routeIdx}] ({routeLength})");
                        if (EditorUtils.FoldoutBegin($"enemy_path__routes[{routeIdx}]__items", $"Steps ({routeLength})")) {
                            for (var itemIdx = 0; itemIdx < routeLength; itemIdx++) {
                                EditorGUILayout.LabelField($"[{itemIdx}]", route.Get(itemIdx).ToString());
                            }
                            EditorUtils.FoldoutEnd();
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                
                EditorUtils.FoldoutEnd();
            }
        }
#endif
    }
}
