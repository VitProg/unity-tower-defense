using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.features._common.components;
using td.features._common.flags;
using td.features.state;
using td.utils;
using td.utils.ecs;
using UnityEngine;

namespace td.features._common.systems
{
    public class MovementToTargetSystem : EcsIntervalableRunSystem
    {
        private readonly EcsWorldInject world;
        private readonly EcsInject<IState> state;
        private readonly EcsInject<Common_Pools> commonPools;
        private readonly EcsInject<Common_Service> common;

        private readonly EcsFilterInject<
            Inc<ObjectTransform, MovementToTarget>,
            Exc<IsSmoothRotation, IsDisabled, IsDestroyed, IsFreezed>
        > filter = default;

        public override void IntervalRun(IEcsSystems systems, float dt)
        {
            // var count = 0u;
            foreach (var entity in filter.Value)
            {
                ref var t = ref filter.Pools.Inc1.Get(entity);
                ref var m = ref filter.Pools.Inc2.Get(entity);

                var correctedDeltaTime = m.speedOfGameAffected
                    ? dt * state.Value.GameSpeed
                    : dt;

                if (!Mathf.Approximately(m.speedV.x, 0f) || !Mathf.Approximately(m.speedV.x, 0f))
                {
                    t.Move(m.speedV.x * correctedDeltaTime, m.speedV.y * correctedDeltaTime);
                    // m.SetSpeed(m.speed, t.rotation);
                }
                else{
                    t.SetPosition(Vector2.MoveTowards(t.position, m.target, correctedDeltaTime * m.speed)); // todo optimize it
                }

                var check = ChechPointIntersections(m.from, m.target, t.position);
                
#if DEBUG && UNITY_EDITOR && MOVEMENT_DEBUG
                var gap = Mathf.Sqrt(m.gapSqr);
                Debug.DrawLine(m.from, m.target, Color.magenta, 0.2f, false);
                Debug.DrawLine(m.from, t.position, Color.yellow, 0.2f, false);
                DebugEx.DrawCross(m.target, gap / 1.1f, Color.cyan, 0.2f);
#endif                    
                
                if(check)
                {
                    commonPools.Value.reachingTargetEventPool.Value.SafeAdd(entity);

                    if (!m.nextTarget.IsZero())
                    {
                        var d = t.position - m.target;
                        var l2 = d.x * d.x + d.y * d.y;
                        
                        // если мы сильно улетели за целевую точку, то надо скорректировать
                        // текущую позицию в соответствии со следующим вектором 
                        if (l2 > m.gapSqr)// 0.005625f)
                        {
                            // todo OPTIMIZE
                            var toNextV = m.nextTarget - m.target;
                            // var toNextL2 = toNextV.x * toNextV.x + toNextV.y * toNextV.y; // L2

                            var l = d.magnitude; // sqrt
                            
                            var vectorToNext = toNextV.normalized * l; // sqrt - _t
                            var correctedPosition = m.target + vectorToNext;

#if DEBUG && UNITY_EDITOR && MOVEMENT_DEBUG
                            Debug.DrawLine(correctedPosition, t.position, Color.red, 0.8f, false);
                            Debug.DrawLine(correctedPosition, m.target, Color.red, 0.8f, false);
#endif
                            t.SetPosition(correctedPosition);
                        }
                    }
                }
            }
        }
        
        private static bool ChechPointIntersections(Vector2 from, Vector2 target, Vector2 current)
        {
            var vectorToFrom = from - target;
            var vectorToCurrent = current - target;

            // Проверим, находятся ли точки с разных сторон окружности относительно target
            var sideFrom = Mathf.Sign(Vector2.Dot(vectorToFrom, vectorToCurrent));
            if (FloatUtils.IsZero(sideFrom))
            {
                // Одна из точек совпадает с центром окружности, вернуть -1
                return false;
            }
            
            // Вычислим проекции точек на направляющий вектор отрезка (from-current)
            var dotCurrentFrom = Vector2.Dot(vectorToFrom, current - from);
            var dotTargetCurrent = Vector2.Dot(vectorToFrom, target - current);
            
            var sideDotFrom = Mathf.RoundToInt(Mathf.Sign(dotCurrentFrom));
            var sideDotCurrent = Mathf.RoundToInt(Mathf.Sign(dotTargetCurrent));
            
            if (sideDotFrom != 0 && sideDotCurrent != 0 && sideDotFrom + sideDotCurrent == 0)
            {
                return true;
            }

            return false;
        }
        
        public MovementToTargetSystem(float interval, float timeShift, Func<float> getDeltaTime) : base(interval, timeShift, getDeltaTime)
        {
        }
    }
}