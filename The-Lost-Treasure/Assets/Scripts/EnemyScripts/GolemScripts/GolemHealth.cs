using UnityEngine;

public class GolemHealth : MonoBehaviour, IDamageable
{
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Die1 = Animator.StringToHash("Die");
    public int health = 100;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= (int)damage;
        
        animator.SetTrigger("Hurt");

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}