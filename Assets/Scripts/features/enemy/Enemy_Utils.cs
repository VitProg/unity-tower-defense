using System.Collections.Generic;
using td.common;
using td.features._common.flags;
using td.features.enemy.components;
using td.features.level.cells;
using td.utils;
using UnityEngine;

namespace td.features.enemy
{
    public static class EnemyUtils
    {
        public static Vector2 CalcPosition(Int2 cellCoordinates, Quaternion rotation, Vector2 offset) =>
            HexGridUtils.CellToPosition(cellCoordinates) + (Vector2)(rotation * offset);
        public static Vector2 CalcPosition(ref Int2 cellCoordinates, Quaternion rotation, Vector2 offset) =>
            HexGridUtils.CellToPosition(ref cellCoordinates) + (Vector2)(rotation * offset);
        public static Vector2 CalcPosition(int cellX, int cellY, Quaternion rotation, Vector2 offset) =>
            HexGridUtils.CellToPosition(cellX, cellY) + (Vector2)(rotation * offset);

        private static readonly Dictionary<HexDirections, Quaternion> AngleQuaternions =
            new()
            {
                [HexDirections.North] =     new Quaternion(0f, 0f, 0f, 1f),
                [HexDirections.NorthEast] = new Quaternion(0f, 0f, -0.5150381f, 0.8571674f),
                [HexDirections.SouthEast] = new Quaternion(0f, 0f, -0.8433915f, 0.5372996f),
                [HexDirections.South] =     new Quaternion(0f, 0f, 1f, 0f),
                [HexDirections.SouthWest] = new Quaternion(0f, 0f, 0.8433915f, 0.5372996f),
                [HexDirections.NorthWest] = new Quaternion(0f, 0f, 0.5150381f, 0.8571674f),
            }; 
        
        //todo optimize
        public static Quaternion LookToNextCell(ref Cell current, ref Cell next)
        {
            // N 0
            // NE -62
            // SE -115
            // S 180
            // SW 115
            // NW 62
            var direction = HexGridUtils.GetDirection(ref current.coords, ref next.coords);
            if (direction == HexDirections.NONE) Debug.DebugBreak();
            return AngleQuaternions[direction];
        }

        public static float GetAngularSpeed(ref Enemy enemy) =>
            enemy.angularSpeed > Constants.Enemy.MinAngularSpeed
                ? enemy.angularSpeed
                : Constants.Enemy.DefaultAngularSpeed;
    }
}