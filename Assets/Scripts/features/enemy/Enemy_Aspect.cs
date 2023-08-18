using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using td.features._common.components;
using td.features.destroy.flags;
using td.features.enemy.components;
using td.features.enemy.flags;
using td.features.enemy.mb;
using td.features.movement.components;
using td.features.movement.flags;
using td.features.movement.systems;

namespace td.features.enemy
{
    public class Enemy_Aspect : ProtoAspectInject, IMovementAspect
    {
        public ProtoPool<Enemy> enemyPool;
        public ProtoPool<IsEnemyDead> isEnemyDeadPool;
        public ProtoPool<Enemy_Path> enemyPathPool;
        public ProtoPool<Ref<EnemyMonoBehaviour>> enemyRefMBPool;
        public ProtoPool<IsTargetReached> isTargetReachedPool;

        public ProtoItExc itLivingEnemies = new ProtoItExc(
            It.Inc<Enemy, ObjectTransform>(),
            It.Exc<IsDestroyed, IsDisabled, IsEnemyDead>()
        );     
        
        public ProtoItExc itMovementEnemies = new(
            It.Inc<Enemy, ObjectTransform, Enemy_Path, Movement>(),
            It.Exc<IsDestroyed, IsDisabled, IsEnemyDead>()
        );

        public ProtoItExc itEnemiesReachingCell = new(
            It.Inc<Enemy, IsTargetReached, Movement, ObjectTransform, Enemy_Path>(),
            It.Exc<IsDestroyed, IsDisabled, IsEnemyDead>()
        );
        
        public ProtoItExc itMovableEnemies = It
            .Chain<Enemy>()
            .Inc<Movement>()
            .Inc<ObjectTransform>()
            .Inc<Enemy_Path>()
            .Exc<IsSmoothRotation>()
            .Exc<IsDestroyed>()
            .Exc<IsDisabled>()
            .Exc<IsHidden>()
            .Exc<IsFreezed>()
            .End();
        /*public ProtoItExc itMovableEnemies = new (
            It.Inc<Enemy, ObjectTransform, Enemy_Path, Movement>(),
            It.Exc(
                typeof(IsSmoothRotation),
                typeof(IsDestroyed),
                typeof(IsDisabled),
                typeof(IsEnemyDead),
                typeof(IsFreezed),
                typeof(IsHidden)
            )
        );*/

        public ProtoItExc GetIt() => itMovableEnemies;

        public ProtoPool<IsTargetReached> GetIsTargetReachedPool() => isTargetReachedPool;
    }
}