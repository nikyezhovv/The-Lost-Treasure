using UnityEngine;

public class MushroomScript : SoundEmitter, IDamageable
{
    [Header("Attack Settings")]
    [SerializeField] public float attackRange = 2;

    [SerializeField] public float attackDamage = 6;
    [SerializeField] public float attackCooldown = 0.8f;

    [Header("Poison Attack Settings")] 
    [SerializeField] public float poisonAttackDamagePerTick = 2;

    [SerializeField] public float poisoningDuration = 2;
    [SerializeField] public float poisoningTickInterval = 0.5f;

    [Header("Health Settings")] 
    [SerializeField] public float maxHealth = 70f;

    private Transform _player;
    private Animator _animator;
    private float _currentHealth;
    private float _lastAttackTime;
    private bool _isHurting;
    private bool _facingRight;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        if (_isHurting) return;

        Flip();

        var distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (Time.time - _lastAttackTime >= attackCooldown)
        {
            _lastAttackTime = Time.time;
            _animator.SetTrigger("Attack");
        }
    }

    private void DealDamage()
    {
        if (Vector2.Distance(transform.position, _player.position) > attackRange) return;

        var playerHealth = _player.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        var roll = Random.value;
        if (roll < 0.6)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            playerHealth.ApplyPoison(poisonAttackDamagePerTick, poisoningDuration, poisoningTickInterval);
        }
    }


    private void Flip()
    {
        Vector2 direction = (_player.position - transform.position).normalized;
        if (direction.x > 0 && !_facingRight || direction.x < 0 && _facingRight)
        {
            _facingRight = !_facingRight;
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void Die()
    {
        PlaySound(sounds[0]);
        _animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
        Destroy(gameObject, 0.8f);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        PlaySound(sounds[1]);
        _animator.SetTrigger("Hurt");
        _isHurting = true;

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke("EndHurtState", 0.4f);
        }
    }

    private void PlayAttackSound()
    {
        PlaySound(sounds[2]);
    }

    private void EndHurtState()
    {
        _isHurting = false;
    }
}