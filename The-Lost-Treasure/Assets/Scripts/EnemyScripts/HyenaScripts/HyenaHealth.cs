/*
using UnityEngine;

public class HyenaHealth : MonoBehaviour, IDamageable
{
    public int health = 40;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // чтобы не получать урон после смерти

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
        animator.SetTrigger("Die"); // ставим триггер анимации
    }

    // Этот метод вызывается из Animation Event на последнем кадре анимации смерти
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}
*/


