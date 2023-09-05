#if UNITY_EDITOR
using UnityEngine.UIElements;
#endif
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.data;
using td.features.eventBus;
using td.features.level.cells;
using td.features.state.interfaces;
using td.utils;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace td.features.level {
    public class Level_State : IStateExtension {
        [DI] private readonly EventBus events;
        private static readonly Type EvType = typeof(Event_Level_StateChanged);
        private Event_Level_StateChanged ev;

#region Private Fields

        private int levelNumber;
        private LevelConfig levelConfig;
        private readonly Slice<Cell> cells = new(128);
        private readonly Dictionary<int, int> cellsDict = new(128);

        private readonly int[] spawns = new int[Constants.Level.MaxSpawns];
        private readonly Dictionary<int, int> spawnsDict = new(Constants.Level.MaxSpawns);

        private readonly int[] kernels = new int[Constants.Level.MaxKernels];
        private readonly Dictionary<int, int> kernelsDict = new(Constants.Level.MaxKernels);

        private int rectXMin = Constants.Level.MaxRect;
        private int rectYMin = Constants.Level.MaxRect;
        private int rectXMax = Constants.Level.MinRect;
        private int rectYMax = Constants.Level.MinRect;

        private Rect rect;
        private Rect rectEx;
        private Vector2 center;
        private bool isPrebuild;

#endregion

#region Getters

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLevelNumber() => levelNumber;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref LevelConfig GetLevelConfig() => ref levelConfig;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSpawnCount() => spawnsDict.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetKernelCount() => kernelsDict.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMapWidth() => rectXMax - rectXMin;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMapHeight() => rectYMax - rectYMin;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect GetRect() => rect;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect GetRectExtra() => rectEx;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 GetCenter() => center;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPrebuild() => isPrebuild;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasSpawn(int number) => spawnsDict.ContainsKey(number);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetSpawn(int number) {
            var index2D = spawns[spawnsDict[number]];
            var idx = cellsDict[index2D];
            return ref cells.Get(idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetSpawnByIndex(int idx) => ref GetCellByIndex2D(spawns[idx]);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKernel(int number) => kernelsDict.ContainsKey(number);

        public ref Cell GetKernel(int number) {
            var index2D = kernels[kernelsDict[number]];
            var idx = cellsDict[index2D];
            return ref cells.Get(idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetKernelByIndex(int idx) => ref GetCellByIndex2D(kernels[idx]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetCell(int2 coords, CellTypes type = CellTypes.Any) => ref GetCell(coords.x, coords.y, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetCell(Vector2 position, CellTypes type = CellTypes.Any) => ref GetCell(HexGridUtils.PositionToCell(position), type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetCell(float x, float y, CellTypes type = CellTypes.Any) => ref GetCell(HexGridUtils.PositionToCell(x, y), type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetCell(int x, int y, CellTypes type = CellTypes.Any) {
            if (isPrebuild) throw new Exception("Can't get cell in pre build mode");
            var index2D = GetIndex2DWithOffset(x, y);
            var idx = cellsDict[index2D];
            ref var cell = ref cells.Get(idx);
            if (type != CellTypes.Any && cell.type != type) throw new Exception($"Type of cell does not match the type {type}");
            return ref cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetCellByIndex2D(int idx) => ref cells.Get(cellsDict[idx]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCell(int2 coords, CellTypes type = CellTypes.Any) => HasCell(coords.x, coords.y, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCell(Vector2 position, CellTypes type = CellTypes.Any) => HasCell(HexGridUtils.PositionToCell(position), type);

        public bool HasCell(int x, int y, CellTypes type = CellTypes.Any) {
            if (isPrebuild) return false;
            var index2D = GetIndex2DWithOffset(x, y);
            if (!cellsDict.ContainsKey(index2D)) return false;
            if (type == CellTypes.Any) return true;
            var idx = cellsDict[index2D];
            ref var cell = ref cells.Get(idx);
            return cell.type == type;
        }

#endregion

#region Setters

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLevelNumber(int number) {
            if (levelNumber == number || number < 0) return;
            levelNumber = number;
            ev.levelNumber = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLevelConfig(ref LevelConfig value) {
            if (levelConfig.levelNumber == levelNumber && levelNumber > 0) return;
            levelConfig = value;
            ev.levelConfig = true;
        }

        public void PreAddCell(Cell cell) {
            cell.isPathAnalyzed = false;

            if (cell.type == CellTypes.Any) throw new Exception("Cell type 'Any' is not supported");

            cells.Add(cell);

            if (cell.coords.x < rectXMin) rectXMin = cell.coords.x;
            if (cell.coords.y < rectYMin) rectYMin = cell.coords.y;
            if (cell.coords.x > rectXMax) rectXMax = cell.coords.x;
            if (cell.coords.y > rectYMax) rectYMax = cell.coords.y;

            if (!isPrebuild) {
                isPrebuild = true;
                ev.isPrebuild = true;
            }
        }

        public void Build() {
            if (!isPrebuild) return;

            var xOffset = rectXMin;
            var yOffset = rectYMin;

            var len = cells.Len();
            for (var idx = 0; idx < len; idx++) {
                ref var cell = ref cells.Get(idx);

                cell.coordsAbs.x = cell.coords.x - xOffset;
                cell.coordsAbs.y = cell.coords.y - yOffset;
                cell.index2D = GetIndex2D(cell.coordsAbs.x, cell.coordsAbs.y);

                if (cellsDict.ContainsKey(cell.index2D)) {
                    Debug.LogWarning($"Cell with coords {cell.coords} already registered!!!");
                    return;
                }

                cellsDict.Add(cell.index2D, idx);

                if (cell.isKernel) {
                    var kernelIdx = kernelsDict.Count;
                    kernelsDict.Add(cell.kernelNumber, kernelIdx);
                    kernels[kernelIdx] = cell.index2D;
                    ev.kernels = true;
                }

                if (cell.isSpawn) {
                    var spawnIdx = spawnsDict.Count;
                    spawnsDict.Add(cell.spawnNumber, spawnIdx);
                    spawns[spawnIdx] = cell.index2D;
                    ev.spawns = true;
                }
            }

            var lt = HexGridUtils.CellToPosition(rectXMax, rectYMin);
            var rt = HexGridUtils.CellToPosition(rectXMax, rectYMax);
            var lb = HexGridUtils.CellToPosition(rectXMin, rectYMin);
            var rb = HexGridUtils.CellToPosition(rectXMax, rectYMin);

            var ltEx = HexGridUtils.CellToPosition(rectXMax - 6, rectYMin + 6);
            var rtEx = HexGridUtils.CellToPosition(rectXMax + 6, rectYMax + 6);
            var lbEx = HexGridUtils.CellToPosition(rectXMin - 6, rectYMin - 6);
            var rbEx = HexGridUtils.CellToPosition(rectXMax + 6, rectYMin - 6);

            // center.x = Math.Abs(rt.x - lt.x) / 2f + Math.Min(rt.x, lt.x);
            // center.y = Math.Abs(lt.y - lb.y) / 2f + Math.Min(lt.y, lb.y);

            rect = new Rect(
                lb.x,
                lb.y,
                rb.x - lb.x,
                rt.y - lb.y
            );

            rectEx = new Rect(
                lbEx.x,
                lbEx.y,
                rbEx.x - lbEx.x,
                rtEx.y - lbEx.y
            );

            center = rect.center;

            isPrebuild = false;

            ev.isPrebuild = true;
            ev.cells = true;
            ev.mapSize = true;
        }

#endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetEventType() => EvType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Refresh() => ev.All();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SendChanges() {
            if (ev.IsEmpty()) return false;
            events.unique.GetOrAdd<Event_Level_StateChanged>() = ev;
            ev = default;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            levelNumber = -1;
            levelConfig = default;
            cells.Clear();
            cellsDict.Clear();
            spawnsDict.Clear();
            kernelsDict.Clear();
            rectXMin = Constants.Level.MaxRect;
            rectYMin = Constants.Level.MaxRect;
            rectXMax = Constants.Level.MinRect;
            rectYMax = Constants.Level.MinRect;
            center = default;
            isPrebuild = true;
            ev.All();
        }

        private int GetIndex2DWithOffset(int x, int y) => (y - rectYMin) * Constants.Level.MaxMapArrayHeight + (x - rectXMin);
        private int GetIndex2D(int x, int y) => y * Constants.Level.MaxMapArrayHeight + x;

#if UNITY_EDITOR
        public string GetStateName() => "Level";

        public void DrawStateProperties(VisualElement root) {
            EditorUtils.DrawProperty("Level Number", levelNumber);
            EditorGUILayout.Space(10);
            EditorUtils.DrawProperty("Is prebuild", isPrebuild);
            EditorGUILayout.Space(10);
            EditorUtils.DrawProperty("Spawns Count", GetSpawnCount());
            EditorUtils.DrawProperty("Kernels Count", GetKernelCount());
            EditorGUILayout.Space(10);
            EditorUtils.DrawProperty("Map Rect", $"[{rectXMin} - {rectXMax}, {rectYMin} - {rectYMax}]");
            EditorUtils.DrawProperty("Width", GetMapWidth());
            EditorUtils.DrawProperty("Height", GetMapHeight());
            EditorGUILayout.Space(10);
            EditorUtils.DrawProperty("Rect", $"[{rect.xMin}..{rect.xMax}; {rect.yMin}..{rect.yMax}]");
            EditorUtils.DrawProperty("Rect Extra", $"[{rectEx.xMin}..{rectEx.xMax}; {rectEx.yMin}..{rectEx.yMax}]");
            EditorUtils.DrawProperty("Center", center);

            if (EditorUtils.FoldoutBegin("level__cells", $"Cells ({cells.Len()})")) {
                if (EditorUtils.FoldoutBegin("level__cells@@walk@@", $"Can Walk Cells")) {
                    for (var idx = 0; idx < cells.Len(); idx++) {
                        ref var cell = ref cells.Get(idx);
                        if (cell.type != CellTypes.CanWalk) continue;
                        DrawCell(ref cell);
                    }
                    EditorUtils.FoldoutEnd();
                }
                EditorGUILayout.Space(5);

                if (EditorUtils.FoldoutBegin("level__cells@@build@@", $"Can Build Cells")) {
                    for (var idx = 0; idx < cells.Len(); idx++) {
                        ref var cell = ref cells.Get(idx);
                        if (cell.type != CellTypes.CanBuild) continue;
                        DrawCell(ref cell);
                    }
                    EditorUtils.FoldoutEnd();
                }
                EditorGUILayout.Space(5);

                if (EditorUtils.FoldoutBegin("level__cells@@other@@", $"Other Cells")) {
                    for (var idx = 0; idx < cells.Len(); idx++) {
                        ref var cell = ref cells.Get(idx);
                        if (cell.type is CellTypes.CanBuild or CellTypes.CanWalk) continue;
                        DrawCell(ref cell);
                    }
                    EditorUtils.FoldoutEnd();
                }
                
                EditorUtils.FoldoutEnd();
            }

            if (EditorUtils.FoldoutBegin("level__spawns", $"Spawns [{GetSpawnCount()}]")) {
                for (var idx = 0; idx < GetSpawnCount(); idx++) {
                    EditorUtils.DrawProperty($"[{idx}]", spawns[idx]);
                }

                EditorUtils.FoldoutEnd();
            }

            if (EditorUtils.FoldoutBegin("level__kernels", $"Kernels [{GetKernelCount()}]")) {
                for (var idx = 0; idx < GetKernelCount(); idx++) {
                    EditorUtils.DrawProperty($"[{idx}]", kernels[idx]);
                }

                EditorUtils.FoldoutEnd();
            }

            if (EditorUtils.FoldoutBegin("level__levelConfig", $"Level Config")) {
                EditorUtils.DrawProperty("Level Number", levelConfig.levelNumber);

                EditorGUILayout.Space(5);

                EditorUtils.DrawProperty("Started Lives", levelConfig.lives);
                EditorUtils.DrawProperty("Started Max Lives", levelConfig.maxLives);

                EditorGUILayout.Space(5);

                EditorUtils.DrawProperty("Started Energy", levelConfig.energy);
                EditorUtils.DrawProperty("Started Max Energy", levelConfig.maxEnergy);

                EditorGUILayout.Space(5);

                EditorUtils.DrawProperty("Before First Wave [s]", levelConfig.delayBeforeFirstWave);
                EditorUtils.DrawProperty("Between Waves [s]", levelConfig.delayBetweenWaves);

                EditorGUILayout.Space(5);

                EditorUtils.DrawProperty("Max Shards", levelConfig.maxShards);

                if (EditorUtils.FoldoutBegin("level__levelConfig.shardStore", "Shards Store")) {
                    EditorUtils.DrawProperty("red", levelConfig.shardsStore.red);
                    EditorUtils.DrawProperty("green", levelConfig.shardsStore.green);
                    EditorUtils.DrawProperty("blue", levelConfig.shardsStore.blue);
                    EditorUtils.DrawProperty("aquamarine", levelConfig.shardsStore.aquamarine);
                    EditorUtils.DrawProperty("yellow", levelConfig.shardsStore.yellow);
                    EditorUtils.DrawProperty("orange", levelConfig.shardsStore.orange);
                    EditorUtils.DrawProperty("pink", levelConfig.shardsStore.pink);
                    EditorUtils.DrawProperty("violet", levelConfig.shardsStore.violet);
                    EditorUtils.FoldoutEnd();
                }

                if (EditorUtils.FoldoutBegin("level__levelConfig.shardsCost", "Shards Price")) {
                    EditorUtils.DrawProperty("red", levelConfig.shardsPrice.red);
                    EditorUtils.DrawProperty("green", levelConfig.shardsPrice.green);
                    EditorUtils.DrawProperty("blue", levelConfig.shardsPrice.blue);
                    EditorUtils.DrawProperty("aquamarine", levelConfig.shardsPrice.aquamarine);
                    EditorUtils.DrawProperty("yellow", levelConfig.shardsPrice.yellow);
                    EditorUtils.DrawProperty("orange", levelConfig.shardsPrice.orange);
                    EditorUtils.DrawProperty("pink", levelConfig.shardsPrice.pink);
                    EditorUtils.DrawProperty("violet", levelConfig.shardsPrice.violet);
                    EditorUtils.FoldoutEnd();
                }

                if (EditorUtils.FoldoutBegin("level__levelConfig.startedShards", "Started Shards")) {
                    for (var index = 0; index < levelConfig.startedShards.Length; index++) {
                        var ss = levelConfig.startedShards[index];
                        EditorUtils.DrawProperty($"[{index}]", index);
                        EditorUtils.DrawProperty("red", ss.red);
                        EditorUtils.DrawProperty("green", ss.green);
                        EditorUtils.DrawProperty("blue", ss.blue);
                        EditorUtils.DrawProperty("aquamarine", ss.aquamarine);
                        EditorUtils.DrawProperty("yellow", ss.yellow);
                        EditorUtils.DrawProperty("orange", ss.orange);
                        EditorUtils.DrawProperty("pink", ss.pink);
                        EditorUtils.DrawProperty("violet", ss.violet);
                        EditorGUILayout.Space(5);
                    }

                    EditorUtils.FoldoutEnd();
                }

                EditorUtils.FoldoutEnd();
            }
        }

        private void DrawCell(ref Cell cell) {
            if (EditorUtils.FoldoutBegin(
                $"level__cells[{cell.index2D}]",
                $"[{cell.index2D}] - [{cell.coords.x}, {cell.coords.y}] {(cell.HasBuilding() ? " [Building]" : "")}{(cell.HasShard() ? " [Shard]" : "")}{(cell.isKernel ? " [Kernel]" : "")}{(cell.isSpawn ? " [Spawn]" : "")}"
            )) {
                EditorUtils.DrawInt2("Coords", cell.coords);
                EditorUtils.DrawInt2("Coords With Offset", cell.coordsAbs);
                EditorUtils.DrawProperty("Index 2D", cell.index2D);

                switch (cell.type) {
                    case CellTypes.CanWalk:
                        EditorUtils.DrawProperty("Is Kernel", cell.isKernel);
                        if (cell.isKernel) EditorUtils.DrawProperty("Kernel Number", cell.kernelNumber);
                        EditorUtils.DrawProperty("Is Spawn", cell.isSpawn);
                        if (cell.isKernel) EditorUtils.DrawProperty("Spawn Number", cell.spawnNumber);
                        EditorUtils.DrawProperty("Dist.Spawn", cell.distanceFromSpawn);
                        EditorUtils.DrawProperty("Dist.Kernel", cell.distanceToKernel);
                        EditorUtils.DrawProperty("Is Auto", cell.isAutoNextSearching);
                        EditorUtils.DrawProperty("Is Analyzed", cell.isPathAnalyzed);
                        if (cell.HasNextDir) EditorUtils.DrawProperty("->", $"{cell.dirToNext} - {HexGridUtils.GetNeighborsCoords(ref cell.coords, cell.dirToNext)}");
                        if (cell.HasNextAltDir) EditorUtils.DrawProperty("=>", $"{cell.dirToNextAlt} - {HexGridUtils.GetNeighborsCoords(ref cell.coords, cell.dirToNextAlt)}");
                        break;
                    case CellTypes.CanBuild:
                        EditorUtils.DrawProperty("Has Building", cell.HasBuilding());
                        EditorUtils.DrawProperty("Has Shard", cell.HasShard());
                        break;
                    default: break;
                }

                EditorUtils.FoldoutEnd();
            }
        }
#endif
    }
}
