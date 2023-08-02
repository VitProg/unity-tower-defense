using NaughtyAttributes;
using td.features.state;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace td.features.enemy.mb
{
    public class CreepEnemyMonoBehaviour : MonoBehaviour
    {
        [Header("Bootstrap")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteLibrary spriteLibrary;
        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] private EnemiesConfig enemiesConfig;

        [Header("Main")]
        [OnValueChanged("UpdateView")][SerializeField] public CreepEnemyTypes type;
        [OnValueChanged("UpdateView")][SerializeField] public CreepEnemyVariants variant;
        
        private static readonly int SType = Animator.StringToHash("Type");

        public void UpdateView()
        {
            spriteLibrary.spriteLibraryAsset = enemiesConfig.GetCreepSprites(type, variant);
            spriteResolver.SetCategoryAndLabel("Run", "1");
            animator.runtimeAnimatorController = enemiesConfig.GetRuntimeAnimatorController(type);
            // animator.speed = (DI.GetCustom<State>()?.GameSpeed ?? 1) * 3;
            // animator.SetInteger(SType, (int)type);
            // animator.Play(((int)type).ToString());
        }

        public static CreepEnemyTypes ParseType(int type) =>
            type switch
            {
                1 => CreepEnemyTypes.Small,
                2 => CreepEnemyTypes.Middle,
                3 => CreepEnemyTypes.Large,
                _ => ParseType(RandomUtils.IntRange(1, 3))
            };
        
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