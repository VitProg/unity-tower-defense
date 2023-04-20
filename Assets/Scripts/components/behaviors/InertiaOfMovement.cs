using System;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UniLeo.Providers;
using td.common;
using UnityEngine;

namespace td.components.behaviors
{
    [Serializable]
    [GenerateProvider]
    public struct InertiaOfMovement
    {
        // public float inertia;
        // [HideInInspector] public Vector2 lastPosition;

        // public Rigidbody2D rb;
        // public float rotationControl;

        public float turnSpeed;
        public float maxEnergy;
        public float energyUsage;
        public float maxSpeed;
        [HideInInspector] public float currentEnergy;
        [HideInInspector] public Vector2 velocity;

        public void AutoReset(ref InertiaOfMovement c)
        {
            c.velocity = Vector2.zero;
            c.currentEnergy = c.maxEnergy;
        }
    }

}