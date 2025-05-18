using UnityEngine;

public class SceletonEnemy : BaseEnemy
{
    [Header("Stun Settings")]
    [SerializeField] public float stunDuration;
    [SerializeField] public float stunDamage;
    
    private readonly System.Random _rnd = new();

    public override void Attack()
    {
        if (Time.time - LastAttackTime >= attackCooldown)
        {
            Rb.linearVelocity = Vector2.zero;
            
            var nextAttackType = _rnd.Next(1, 4);
            LastAttackTime = Time.time;
            if (nextAttackType == 3)
            {
                Animator.SetTrigger("Attack1");
            }
            else
            {
                Animator.SetTrigger("Attack");
            }
        }
    }

    public void Stun()
    {
        // Вызвается из анимации
        if (Vector2.Distance(transform.position, Player.position) <= attackRange)
        {
            var playerHealth = Player.GetComponent<PlayerHealth>();
            var playerMovements = Player.GetComponent<PlayerControls>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(stunDamage);
                playerMovements.Stun(stunDuration);
            }
        }
    }
}