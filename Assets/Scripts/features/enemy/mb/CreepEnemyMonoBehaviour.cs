using System.Runtime.CompilerServices;
using NaughtyAttributes;
using td.features.enemy.data;
using td.utils;
using UnityEngine;
using UnityEngine.U2D.Animation; /* todo Doues it's works in runtime? */

namespace td.features.enemy.mb
{
    public class CreepEnemyMonoBehaviour : MonoBehaviour
    {
        [Header("Bootstrap")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteLibrary spriteLibrary;
        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] private Enemies_Config_SO enemiesConfigSO;

        [Header("Main")]
        [OnValueChanged("UpdateView")][SerializeField] public CreepEnemyTypes type;
        [OnValueChanged("UpdateView")][SerializeField] public CreepEnemyVariants variant;
        
        private static readonly int SType = Animator.StringToHash("Type");

        public void UpdateView()
        {
            spriteLibrary.spriteLibraryAsset = enemiesConfigSO.GetCreepSprites(type, variant);
            spriteResolver.SetCategoryAndLabel("Run", "1");
            animator.runtimeAnimatorController = enemiesConfigSO.GetRuntimeAnimatorController(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CreepEnemyTypes ParseType(int type) =>
            type switch
            {
                1 => CreepEnemyTypes.Small,
                2 => CreepEnemyTypes.Middle,
                3 => CreepEnemyTypes.Large,
                _ => ParseType(RandomUtils.IntRange(1, 3))
            };
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CreepEnemyVariants ParseVariant(int variant) =>
            variant switch
            {
                1 => CreepEnemyVariants.Blue,
                2 => CreepEnemyVariants.Green,
                3 => CreepEnemyVariants.Red,
                4 => CreepEnemyVariants.Yellow,
                _ => ParseVariant(RandomUtils.IntRange(1, 4))
            };
    }

}