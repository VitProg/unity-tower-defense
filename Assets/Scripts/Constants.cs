using UnityEngine;

namespace td
{
    public static class Constants
    {
        public const float DefaultGap = 0.2f;
        public const float ZeroFloat = 0.0001f;
        
        public static class Enemy
        {
            public const uint DeltaRespawnRect = 10; 
            public const uint MaxEnemies = 100;
            public const float DefaultSpeed = 1.0f;
            public const float MinSpeed = 10.5f;
            public const float MaxSpeed = 20.0f;
            public const float MinSize = 0.4f;
            public const float MaxSize = 0.5f;
            public const float OffsetMin = -0.15f;
            public const float OffsetMax = 0.15f;

            public const float DefaultAngularSpeed = 5f;
            public const float MinAngularSpeed = 0.01f;
            public const float SmoothRotationThreshold = 100f;
        }
        
        public static class Tags
        {
            public const string Enemy = "Enemy";
            public const string EnemiesContainer = "EnemiesContainer";
            public const string BuildingsContainer = "BuildingsContainer";
            public const string Building = "Building";
            public const string Spawn = "Spawn";
            public const string Target = "Target";
            public const string Tile = "Tile";
            public const string Projectile = "projectile";
            public const string CanWalk = "CanWalk";
            public const string CanBuild = "CanBuild";
            public const string GridRenderer = "GridRenderer";
        }
        
        public static class Level
        {
            public const uint MaxMapArrayWidth = 100;
            public const uint MaxMapArrayHeight = 100;
            public const uint MaxSpawns = 10;
            public const uint MaxTargets = 5;
        }
        
        public static class Worlds
        {
            public const string Default = default;
            public const string Outer = "outer";
            public const string UI = "ui";
        }
        
        public static class Camera
        {
            public const float MaxOrthographicZoom = 1f;
            public const float MinOrthographicZoom = 6.5f;
            public const float OrthographicZoomStep = 0.5f;
            public const float OrthographicZoomSpeed = 5f;
            
            public const float MaxPerspectiveZoom = -4f;
            public const float MinPerspectiveZoom = -20f;
            public const float PerspectiveZoomStep = 1f;
            public const float PerspectiveZoomSpeed = 5f;
            

            public const float MoveSpeedMouse  = 3.0f;
            public const float MaxMoveSpeedMouse  = 10.0f;
            
            public const float MoveSpeedKeyborad = 25f;
            
            public const float MoveMouseAttenuation = 0.8f;
            public const float MoveMouseInertiatiaAttenuation = 0.8f;
            public const float MoveKeyboardInertiatiaAttenuation = 0.8f;
            
            public const float MaxMoveSpeed = .5f;
        }

        public static class UI
        {
            public static class DragNDrop
            {
                public const double TimeForAwaitDown = 0.1d;
                public const bool Smooth = true;
                public const float SmoothSpeed = 15f;

            }

            public static class Components
            {
                public const string LivesLabel = "LivesLabel";
                public const string MoneyLabel = "MoneyLabel";
                public const string WaveCountdownLabel = "WaveCountdownLabel";
                public const string WaveCountdown = "WaveCountdown";
                public const string WaveLabel = "WaveLabel";
                public const string EnemiesLabel = "EnemiesLabel";

                public const string AddTowerButton = "AddTower";
            }
        }

        public static class Pools
        {
            public const int ProjectileDefaultCopacity = 50;
            public const int ProjectileMaxCopacity = 100;
            public const int EnemyDefaultCopacity = 20;
            public const int EnemyMaxCopacity = 100;
        }
    }
}