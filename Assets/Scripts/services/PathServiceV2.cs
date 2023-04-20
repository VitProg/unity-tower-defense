using System.Collections.Generic;
using td.common;
using td.monoBehaviours;
using td.utils;
using UnityEngine;

namespace td.services
{
    public class PathServiceV2 : IPathService
    {
        private readonly Queue<Cell> queue = new();
        private readonly Queue<Cell> idleQueue = new();

        private LevelMap levelMap;

        private static readonly HexDirections[] DefaultDirections = new []
        {
            HexDirections.NorthWest,
            HexDirections.North,
            HexDirections.NorthEast,
            HexDirections.SouthEast,
            HexDirections.South,
            HexDirections.SouthWest
        };

        public void InitPath(LevelMap levelMap)
        {
            this.levelMap = levelMap;

            queue.Clear();

            Debug.Assert(levelMap.Spawns != null);
            var spawns = levelMap.Spawns;

            uint spawnIndex = 1;
            foreach (var spawn in spawns)
            {
                var spawnCell = levelMap.GetCell(spawn, CellTypes.CanWalk);
                if (!spawnCell) continue;
                spawnCell.spawnNumber = spawnIndex;
                spawnCell.distanceFromSpawn = 0;
                queue.Enqueue(spawnCell);
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

        private void Tick(Cell currentCell)
        {
            var directions = new List<HexDirections>();
            
            if (currentCell.isPathAnalyzed || currentCell.isKernel) return;

            if (
                !currentCell.isAutoNextSearching &&
                (
                    (currentCell.isSwitcher && currentCell.HasDirectionToNext && currentCell.HasAltSirectionToNext) ||
                    (!currentCell.isSwitcher && currentCell.HasDirectionToNext)
                )
            )
            {
                directions.Add(currentCell.directionToNext);
                if (currentCell.isSwitcher)
                {
                    directions.Add(currentCell.directionToAltNext);
                }
            }
            else
            {
                directions.AddRange(DefaultDirections);
            }

            currentCell.directionToNext = HexDirections.NONE;
            currentCell.directionToAltNext = HexDirections.NONE;

            foreach (var direction in directions)
            {
                var nextCell = levelMap.GetCell(HexGridUtils.GetNeighborsCoords(currentCell.Coords, direction), CellTypes.CanWalk);
                if (nextCell == null) continue;

                // if ((nextCell.Coords.x == 24 && nextCell.Coords.y == 5) ||
                    // (currentCell.Coords.x == 24 && currentCell.Coords.y == 5) || nextCell.Coords.y >= 5 || currentCell.Coords.y >= 5) 
                // {
                    // Debug.Break(); 
                // }
                
                // if we look at the cell from which we came, we skip it
                if (nextCell.isPathAnalyzed)
                {
                    if ((nextCell.NextCoords == currentCell.Coords) ||
                        nextCell.isSwitcher && nextCell.AltNextCoords == currentCell.Coords )
                    {
                        continue;
                    }
                }

                if (currentCell.isSwitcher)
                {
                    if (currentCell.HasDirectionToNext == false)
                    {
                        currentCell.directionToNext = direction;
                        nextCell.distanceFromSpawn = currentCell.distanceFromSpawn + 1;
                        nextCell.directionToPrev = HexGridUtils.ReverseDirection(direction);
                        idleQueue.Enqueue(nextCell);
                    }
                    else if (currentCell.HasAltSirectionToNext == false)
                    {
                        currentCell.directionToAltNext = direction;
                        nextCell.distanceFromSpawn = currentCell.distanceFromSpawn + 1;
                        nextCell.directionToPrev = HexGridUtils.ReverseDirection(direction);
                        idleQueue.Enqueue(nextCell);
                    }
                }
                else
                {
                    if (currentCell.HasDirectionToNext == false)
                    {
                        currentCell.directionToNext = direction;
                        nextCell.distanceFromSpawn = currentCell.distanceFromSpawn + 1;
                        nextCell.directionToPrev = HexGridUtils.ReverseDirection(direction);
                        queue.Enqueue(nextCell);
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
            
            currentCell.isPathAnalyzed = true;

            if (
                (currentCell.isSwitcher && !currentCell.HasDirectionToNext && !currentCell.HasAltSirectionToNext) ||
                (!currentCell.isSwitcher && !currentCell.HasDirectionToNext)
            )
            {
                ClearBadPath(currentCell);
            }
        }

        private void ClearBadPath(Cell currentCell)
        {
            if (currentCell.directionToPrev != HexDirections.NONE && !currentCell.isKernel)
            {
                var reversedPrevDirection = HexGridUtils.ReverseDirection(currentCell.directionToPrev);
                
                var prevCoords = HexGridUtils.GetNeighborsCoords(currentCell.Coords, currentCell.directionToPrev);
                var prevCell = levelMap.GetCell(prevCoords, CellTypes.CanWalk);

                if (prevCell && prevCell.isPathAnalyzed)
                {
                    var deep = false;
                    if (prevCell.directionToNext == reversedPrevDirection)
                    {
                        prevCell.directionToNext = HexDirections.NONE;
                        deep = true;
                    }

                    if (prevCell.directionToAltNext == reversedPrevDirection)
                    {
                        prevCell.directionToAltNext = HexDirections.NONE;
                        deep = true;
                    }

                    if (deep && !prevCell.isSwitcher)
                    {
                        ClearBadPath(prevCell);
                    }
                }
            }
        }
    }
}