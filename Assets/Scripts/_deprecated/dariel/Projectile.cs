using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int attackStrength;

    [SerializeField] private ProjectileType projectileType;

    public int AttackStrength => attackStrength;

    public ProjectileType ProjectileType => projectileType;
}

public enum ProjectileType
{
    rock,
    arrow,
    fireball
}