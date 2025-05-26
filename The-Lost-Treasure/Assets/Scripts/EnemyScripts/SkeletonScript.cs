using UnityEngine;

public class SkeletonEnemy : BaseEnemy
{
    [Header("Stun Settings")] 
    [SerializeField] public float stunDuration = 2f;
    [SerializeField] public float stunDamage = 5f;

    public override void Attack()
    {
        if (Time.time - LastAttackTime < attackCooldown)
            return;

        Rb.linearVelocity = Vector2.zero;
        LastAttackTime = Time.time;
        
        Animator.SetTrigger(GetRandomAttackTrigger());
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

    private void PlaySwordAttackSound()
    {
        PlaySound(sounds[3]);
    }
    private void PlayShieldAttackSound()
    {
        PlaySound(sounds[4]);
    }

    private string GetRandomAttackTrigger()
    {
        return Random.value < 0.4f ? "Attack" : "Attack1";
    }
}