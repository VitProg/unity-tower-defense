using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [SerializeField]
    private int healthPoints;
    private int target = 0;
    private Transform enemy;
    private Collider2D enemyCollider;
    private Animator anim;
    private float navigationTime = 0;
    private bool isDead = false;

    public bool IsDead
    {
        get { return isDead; }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.tag == "checkpoint")
            target += 1;
        else if (collider2D.tag == "Finish")
        {
            //GameManager.Instance.RoundEscaped += 1;
            //GameManager.Instance.TotalEscape += 1;
            //GameManager.Instance.UnregisterEnemy(this);
            //GameManager.Instance.isWaveOver();
        }
        else if (collider2D.gameObject.tag == "projectile")
        {
            //Projectile projectile = collider2D.gameObject.GetComponent<Projectile>();
            enemyHit(collider2D.gameObject.GetComponent<Projectile>().AttackStrength);
            Destroy(collider2D.gameObject);
        }
    }
    public void enemyHit(int hitPoints)
    {
        if (healthPoints - hitPoints > 0)
        {
            healthPoints -= hitPoints;
            //anim.Play("Hurt");
            //GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
        }
        else
        {
            //anim.SetTrigger("didDie");
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        enemyCollider.enabled = false;
        //GameManager.instance.TotalKilled += 1;
        //GameManager.instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
        //GameManager.instance.AddMoney(rewardAmount);
        //GameManager.instance.isWaveOver();
    }
}
