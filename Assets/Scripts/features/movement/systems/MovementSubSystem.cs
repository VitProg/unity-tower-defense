using System;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using Leopotam.Types;
using td.features.movement.flags;
using td.features.state;
using td.utils;
using td.utils.ecs;
using Unity.Mathematics;
using UnityEngine;

namespace td.features.movement.systems
{
    public interface IMovementAspect
    {
        public ProtoItExc GetIt();
        public ProtoPool<IsTargetReached> GetIsTargetReachedPool();
    }
    
    public class MovementSubSystem
    {
        private readonly IMovementAspect aspect;
        [DI] private Movement_Service movementService;
        [DI] private State state;
        
        public MovementSubSystem(IMovementAspect aspect)
        {
            this.aspect = aspect;
        }

        public void Run(float deltaTime)
        {
            if (!state.GetSimulationEnabled()) return;
            
            // var count = 0u;
            foreach (var entity in aspect.GetIt())
            {
                if (!movementService.HasTransform(entity) || !movementService.HasMovement(entity))
                {
#if DEBUG
                    Debug.LogWarning($"MovementToTargetSystem: Entity {entity} has no transform or movement component! Check you filter!");
#endif
                    continue;
                }
                
                ref var t = ref movementService.GetTransform(entity);
                ref var m = ref movementService.GetMovement(entity);

                var correctedDeltaTime = m.speedOfGameAffected
                    ? deltaTime * state.GetGameSpeed()
                    : deltaTime;

                if (!FloatUtils.IsZero(m.speedV.x) || !FloatUtils.IsZero(m.speedV.x))
                {
                    t.Move(m.speedV.x * correctedDeltaTime, m.speedV.y * correctedDeltaTime);
                    // m.SetSpeed(m.speed, t.rotation);
                }
                else{
                    t.SetPosition(Vector2.MoveTowards(t.position, m.target, correctedDeltaTime * m.speed)); // todo optimize it
                }

                var check = ChechPointIntersections(m.from, m.target, t.position);
                
#if DEBUG && UNITY_EDITOR && MOVEMENT_DEBUG
                var gap = (float) Math.Sqrt(m.gapSqr);
                Debug.DrawLine(m.from, m.target, Color.magenta, 0.2f, false);
                Debug.DrawLine(m.from, t.position, Color.yellow, 0.2f, false);
                DebugEx.DrawCross(m.target, gap / 1.1f, Color.cyan, 0.2f);
#endif                    
                
                if(check)
                {
                    aspect.GetIsTargetReachedPool().GetOrAdd(entity);

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

                            var l = d.Magnitude(); // sqrt
                            
                            var vectorToNext = math.normalize(toNextV) * l; // sqrt - _t
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
            var sideFrom = MathFast.Sign(Vector2.Dot(vectorToFrom, vectorToCurrent));
            if (FloatUtils.IsZero(sideFrom))
            {
                // Одна из точек совпадает с центром окружности, вернуть -1
                return false;
            }
            
            // Вычислим проекции точек на направляющий вектор отрезка (from-current)
            var dotCurrentFrom = Vector2.Dot(vectorToFrom, current - from);
            var dotTargetCurrent = Vector2.Dot(vectorToFrom, target - current);
            
            var sideDotFrom = MathFast.Round(MathFast.Sign(dotCurrentFrom));
            var sideDotCurrent = MathFast.Round(MathFast.Sign(dotTargetCurrent));
            
            if (sideDotFrom != 0 && sideDotCurrent != 0 && sideDotFrom + sideDotCurrent == 0)
            {
                return true;
            }

            return false;
        }
    }
}