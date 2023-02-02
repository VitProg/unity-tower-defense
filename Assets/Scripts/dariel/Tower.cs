using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private float _attackRange;
    [SerializeField]
    private float _timeBetweenAttacks;
    private bool _isAttacking = false;
    private float _attackingTime;
    private GameObject _targetEnemy = null;
    [SerializeField]
    private GameObject _projectile;
    private Structures structures = Structures.SimpleTower;
    void Start()
    {

    }

    void Update()
    {
        
        _attackingTime -= Time.deltaTime;
        if (_targetEnemy == null || _targetEnemy.GetComponent<SimpleEnemy>().IsDead)
        {
            _targetEnemy = GetClosestEnemyInRange();
            Debug.Log("NAME:=" + _targetEnemy?.name);
        }
        else
        {
            if (_attackingTime <= 0f)
            {
                _isAttacking = true;
                _attackingTime = _timeBetweenAttacks;
            }
            else
            {
                _isAttacking = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    public Structures GetStructureType()
    {
        return structures;
    }

    void FixedUpdate()
    {
        if (_isAttacking) { Attack(); }

    }

    private GameObject GetClosestEnemyInRange()
    {
        Debug.Log("@@@@@@@@GetClosestEnemyInRange");
        GameObject closestEnemy = null;
        float smallestDistance = float.PositiveInfinity;
        List<GameObject> allEnemies = GameManager.instance.GetAllEnemies();
        for (int i = 0; i < allEnemies.Count; i++)
        {
            float distantion = Vector2.Distance(transform.localPosition, allEnemies[i].transform.localPosition);
            if (distantion <= _attackRange)
            {
                if (distantion < smallestDistance)
                {
                    smallestDistance = distantion;
                    closestEnemy = allEnemies[i].gameObject;
                }
            }
            /*
            if (distantion < smallestDistance & distantion <= _attackRange)
            {
                smallestDistance = distantion;
                closestEnemy = allEnemies[i].gameObject;
            }
            */
            i++;
        }
        if(closestEnemy != null)
        {
            Debug.Log($"ENEMYx: {closestEnemy.transform.position.x} /y: {closestEnemy.transform.position.y}");
        }
        return closestEnemy;
    }

    public void Attack()
    {
        _isAttacking = false;
        GameObject newProjectile = Instantiate(_projectile, transform.position, Quaternion.identity, transform);
        //newProjectile.transform.localPosition = transform.localPosition;

        if (newProjectile.GetComponent<Projectile>().ProjectileType == ProjectileType.arrow)
        {
            //GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Arrow);
        }
        else if (newProjectile.GetComponent<Projectile>().ProjectileType == ProjectileType.fireball)
        {
            //GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Fireball);
        }
        else if (newProjectile.GetComponent<Projectile>().ProjectileType == ProjectileType.rock)
        {
            //GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Rock);
        }
        //If we have a target enemy, start a coroutine to shoot projectile to target enemy
        if (_targetEnemy == null)
        {
            Destroy(newProjectile);
        }
        else
        {
            StartCoroutine(MoveProjectile(newProjectile));
        }
    }

    IEnumerator MoveProjectile(GameObject projectile)
    {
        Vector3 finalAttackPoint = _targetEnemy.transform.position;
        while (Vector2.Distance(projectile.transform.localPosition, finalAttackPoint) > 0.20f && projectile != null && _targetEnemy != null)
        {
            var dir = _targetEnemy.transform.localPosition - transform.localPosition;
            var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                         //Angle of the projectile
            projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);  //Rotation of projectile
            //projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition, finalAttackPoint, 5f * Time.deltaTime); //Move Projectile
            projectile.transform.position = Vector2.MoveTowards(projectile.transform.position, finalAttackPoint, 5f * Time.deltaTime); //Move Projectile
            yield return null;
        }
        if (projectile != null || _targetEnemy == null)
        {
            Destroy(projectile);
        }
    }
}
