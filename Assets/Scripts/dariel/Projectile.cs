using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private int attackStrength;

    [SerializeField]
    private ProjectileType projectileType;

    public int AttackStrength
    {
        get { return attackStrength; }
    }

    public ProjectileType ProjectileType
    {
        get { return projectileType; }
    }
}

public enum ProjectileType
{
    rock,
    arrow,
    fireball
};