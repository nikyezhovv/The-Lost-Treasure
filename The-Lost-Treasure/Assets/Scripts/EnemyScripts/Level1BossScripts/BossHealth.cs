using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{

    public int health = 500;
    public GameObject deathEffect;
    public bool isInvulnerable;

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;
        
        Debug.Log($"Boss took {damage} damage. {health}HP remaining");
        health -= (int)damage;

        if (health <= 200)
        {
            GetComponent<Animator>().SetBool("IsEnraged", true);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}