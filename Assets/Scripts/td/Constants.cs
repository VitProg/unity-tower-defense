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
            public const float OffsetMin = -0.5f;
            public const float OffsetMax = 0.5f;
        }
        
        public static class Tags
        {
            public const string Enemy = "Enemy";
            public const string EnemiesContainer = "EnemiesContainer";
            public const string Spawn = "Spawn";
            public const string Target = "Target";
            public const string Tile = "Tile";
        }
        
        public static class Level
        {
            public const uint MaxWidth = 50;
            public const uint MaxHeight = 50;
            public const uint MaxSpawns = 10;
            public const uint MaxTargets = 5;
        }
        
        public static class Ecs
        {
            public const string EventWorldName = "events";
        }
    }
}