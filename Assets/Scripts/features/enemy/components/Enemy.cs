using System;
using Leopotam.EcsLite;
using td.features._common;
using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.enemy.components
{
    [Serializable]
    public struct Enemy : IEcsAutoReset<Enemy>
    {
        public const string Type = "enemy";
        
        public void AutoReset(ref Enemy c)
        {
            c = default;
            c._id_ = CommonUtils.ID(Type);
        }
        
        // ReSharper disable once InconsistentNaming
        public uint _id_;
        public string enemyName;

        public float distanceToKernel;
        public float distanceFromSpawn;

        public Vector2 offset;
        
        public int spawmNumber;
        public float startingSpeed;
        public float startingHealth;
        public float angularSpeed;
        
        //EnemyState
        public float speed;
        public float health;
        public float damage;
        
        public uint energy;

        public override string ToString()
        {
            return $"{enemyName}#{_id_}: hp{health}, spd{speed}, dmg{damage}, shp{startingHealth}, sspd{startingSpeed}";
        }
    }
}