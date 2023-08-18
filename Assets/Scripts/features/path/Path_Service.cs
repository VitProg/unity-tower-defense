using System.Collections.Generic;
using Leopotam.EcsProto.QoL;
using td.features.level;
using td.features.level.cells;
using td.utils;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.path
{
    public class Path_Service
    {
        [DI] private LevelMap levelMap;
        
        private readonly Queue<int2> queue = new();
        private readonly Queue<int2> idleQueue = new();

        private static readonly HexDirections[] DefaultDirections = new []
        {
            HexDirections.NorthWest,
            HexDirections.North,
            HexDirections.NorthEast,
            HexDirections.SouthEast,
            HexDirections.South,
            HexDirections.SouthWest
        };

        public void InitPath()
        {
            queue.Clear();

            Debug.Assert(levelMap.spawns != null && levelMap.SpawnCount > 0);

            for (var spawnIndex = 0; spawnIndex < levelMap.SpawnCount; spawnIndex++)
            {
                var spawnCoords = levelMap.spawns[spawnIndex];
                if (!spawnCoords.HasValue || !levelMap.HasCell(spawnCoords.Value.x, spawnCoords.Value.y, CellTypes.CanWalk)) continue;
                ref var spawnCell = ref levelMap.GetCell(spawnCoords.Value.x, spawnCoords.Value.y, CellTypes.CanWalk);
                spawnCell.spawnNumber = (byte)spawnIndex;
                spawnCell.distanceFromSpawn = 0;
                queue.Enqueue(spawnCell.coords);
                spawnIndex++;
            }

            if (queue.Count <= 0)
            {
                Debug.LogWarning("Spawns Not Founds!");
            }
            else
            {
                do
                {
                    do
                    {
                        Tick(queue.Dequeue());
                    } while (queue.Count > 0);

                    foreach (var cell in idleQueue)
                    {
                        queue.Enqueue(cell);
                    }

                    idleQueue.Clear();
                } while (queue.Count > 0);
            }

            queue.Clear();

            //todo add step for calculate distanceToKernel
        }

        private void Tick(int2 coords)
        {
            if (!levelMap.HasCell(coords.x, coords.y)) return;

            ref var cell = ref levelMap.GetCell(coords.x, coords.y);
            
            var directions = new List<HexDirections>();
            
            if (cell.isPathAnalyzed || cell.isKernel) return;

            if (
                !cell.isAutoNextSearching &&
                cell is 
                    { isSwitcher: true, HasNextDir: true, HasNextAltDir: true } or
                    { isSwitcher: false, HasNextDir: true }
            )
            {
                directions.Add(cell.dirToNext);
                if (cell.isSwitcher)
                {
                    directions.Add(cell.dirToNextAlt);
                }
            }
            else
            {
                directions.AddRange(DefaultDirections);
            }

            cell.dirToNext = HexDirections.NONE;
            cell.dirToNextAlt = HexDirections.NONE;

            foreach (var direction in directions)
            {
                var nCoords = HexGridUtils.GetNeighborsCoords(ref cell.coords, direction);
                if (!levelMap.HasCell(nCoords.x, nCoords.y, CellTypes.CanWalk)) continue;
                ref var nextCell = ref levelMap.GetCell(nCoords.x, nCoords.y, CellTypes.CanWalk);
                
                // if we look at the cell from which we came, we skip it
                if (nextCell.isPathAnalyzed)
                {
                    if (
                        cell.coords.Equals(HexGridUtils.GetNeighborsCoords(ref nextCell.coords, nextCell.dirToNext)) ||
                        nextCell.isSwitcher && cell.coords.Equals(HexGridUtils.GetNeighborsCoords(ref nextCell.coords, nextCell.dirToNextAlt))
                    )
                    {
                        continue;
                    }
                }

                if (cell.isSwitcher)
                {
                    if (cell.HasNextDir == false)
                    {
                        cell.dirToNext = direction;
                        nextCell.distanceFromSpawn = (ushort)(cell.distanceFromSpawn + 1);
                        nextCell.dirToPrev = HexGridUtils.ReverseDirection(direction);
                        idleQueue.Enqueue(nextCell.coords);
                    }
                    else if (cell.HasNextAltDir == false)
                    {
                        cell.dirToNextAlt = direction;
                        nextCell.distanceFromSpawn = (ushort)(cell.distanceFromSpawn + 1);
                        nextCell.dirToPrev = HexGridUtils.ReverseDirection(direction);
                        idleQueue.Enqueue(nextCell.coords);
                    }
                }
                else
                {
                    if (cell.HasNextDir == false)
                    {
                        cell.dirToNext = direction;
                        nextCell.distanceFromSpawn = (ushort)(cell.distanceFromSpawn + 1);
                        nextCell.dirToPrev = HexGridUtils.ReverseDirection(direction);
                        queue.Enqueue(nextCell.coords);
                    }
                    else
                    {
                        // Debug.Log("SOMOTHING STREINGHT!!!");
                        // throw new InvalidDataException();
                        //ToDo
                        // idleQueue.Enqueue(nextCell);
                    }
                }
            }
            
            cell.isPathAnalyzed = true;

            if (
                (cell is { isSwitcher: true, HasNextDir: false, HasNextAltDir: false }) ||
                cell is { isSwitcher: false, HasNextDir: false }
            )
            {
                ClearBadPath(ref cell);
            }
        }

        private void ClearBadPath(ref Cell cell)
        {
            if (cell.dirToPrev == HexDirections.NONE || cell.isKernel) return;
            
            var reversedPrevDirection = HexGridUtils.ReverseDirection(cell.dirToPrev);
                
            var prevCoords = HexGridUtils.GetNeighborsCoords(ref cell.coords, cell.dirToPrev);
                
            if (!levelMap.HasCell(prevCoords, CellTypes.CanWalk)) return;
                
            ref var prevCell = ref levelMap.GetCell(prevCoords.x, prevCoords.y, CellTypes.CanWalk);

            if (!prevCell.isPathAnalyzed) return;
            
            var deep = false;
            if (prevCell.dirToNext == reversedPrevDirection)
            {
                prevCell.dirToNext = HexDirections.NONE;
                deep = true;
            }

            if (prevCell.dirToNextAlt == reversedPrevDirection)
            {
                prevCell.dirToNextAlt = HexDirections.NONE;
                deep = true;
            }

            if (deep && !prevCell.isSwitcher)
            {
                ClearBadPath(ref prevCell);
            }
        }
    }
}