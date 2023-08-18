using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace td.features.enemy.data
{
    [CreateAssetMenu(menuName = "TD/EnemiesConfig")]
    public class Enemies_Config_SO : ScriptableObject
    {
        [Header("Creep Enemy")]
        [Header("* Small")]
        public SpriteLibraryAsset creepSmallBlueSprites;
        public SpriteLibraryAsset creepSmallGreenSprites;
        public SpriteLibraryAsset creepSmallRedSprites;
        public SpriteLibraryAsset creepSmallYellowSprites;
        public RuntimeAnimatorController creepSmallAnimatorController;
        
        [Header("* Middle")]
        public SpriteLibraryAsset creepMiddleBlueSprites;
        public SpriteLibraryAsset creepMiddleGreenSprites;
        public SpriteLibraryAsset creepMiddleRedSprites;
        public SpriteLibraryAsset creepMiddleYellowSprites;
        public RuntimeAnimatorController creepMiddleAnimatorController;
        
        [Header("* Large")]
        public SpriteLibraryAsset creepLargeBlueSprites;
        public SpriteLibraryAsset creepLargeGreenSprites;
        public SpriteLibraryAsset creepLargeRedSprites;
        public SpriteLibraryAsset creepLargeYellowSprites;
        public RuntimeAnimatorController creepLargeAnimatorController;

        public RuntimeAnimatorController GetRuntimeAnimatorController(CreepEnemyTypes type)
        {
            return type switch
            {
                CreepEnemyTypes.Small => creepSmallAnimatorController,
                CreepEnemyTypes.Middle => creepMiddleAnimatorController,
                CreepEnemyTypes.Large => creepLargeAnimatorController,
#if UNITY_EDITOR
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
#endif
            };
        }

        public SpriteLibraryAsset GetCreepSprites(CreepEnemyTypes type, CreepEnemyVariants variant)
        {
            switch (type)
            {
                case CreepEnemyTypes.Small:
                    return variant switch
                    {
                        CreepEnemyVariants.Blue => creepSmallBlueSprites,
                        CreepEnemyVariants.Green => creepSmallGreenSprites,
                        CreepEnemyVariants.Red => creepSmallRedSprites,
                        CreepEnemyVariants.Yellow => creepSmallYellowSprites,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                case CreepEnemyTypes.Middle:
                    return variant switch
                    {
                        CreepEnemyVariants.Blue => creepMiddleBlueSprites,
                        CreepEnemyVariants.Green => creepMiddleGreenSprites,
                        CreepEnemyVariants.Red => creepMiddleRedSprites,
                        CreepEnemyVariants.Yellow => creepMiddleYellowSprites,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                case CreepEnemyTypes.Large:
                    return variant switch
                    {
                        CreepEnemyVariants.Blue => creepLargeBlueSprites,
                        CreepEnemyVariants.Green => creepLargeGreenSprites,
                        CreepEnemyVariants.Red => creepLargeRedSprites,
                        CreepEnemyVariants.Yellow => creepLargeYellowSprites,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    
    public enum CreepEnemyTypes
    {
        Small = 1,
        Middle = 2,
        Large = 3
    }

    public enum CreepEnemyVariants
    {
        Blue = 1,
        Green = 2,
        Red = 3,
        Yellow = 4,
    }
}