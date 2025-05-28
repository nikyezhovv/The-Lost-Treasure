using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

public class BaseFlyingEnemy : Sounds, IDamageable
{
    [Header("Detection & Attack")] [SerializeField]
    private float activationRange = 8f;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Health")] [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;

    private Transform _player;
    private Animator _animator;
    private Seeker _seeker;
    private AIPath _aiPath;
    private AIDestinationSetter _aiDestinationSetter;
    private float _lastAttackTime;
    private bool _isHurting;
    private bool _isAIActive;
    private bool _facingRight;
    private PlayerHealth _playerHealth;


    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _seeker = GetComponent<Seeker>();
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        _aiPath = GetComponent<AIPath>();
        currentHealth = maxHealth;
        _playerHealth = _player.GetComponent<PlayerHealth>();

        if (_aiPath != null && _seeker != null && _aiDestinationSetter != null)
        {
            ChangeAIState(isActive: false);
        }
    }

    private void ChangeAIState(bool isActive)
    {
        if (isActive)
        {
            _aiPath.enabled = true;
            _aiDestinationSetter.enabled = true;
            _seeker.enabled = true;
            _isAIActive = true;
        }
        else
        {
            _aiPath.enabled = false;
            _aiDestinationSetter.enabled = false;
            _seeker.enabled = false;
            _isAIActive = false;
        }
    }

    void Update()
    {
        if (_player == null || _isHurting) return;

        var distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        Flip();

        if (distanceToPlayer <= activationRange && !_playerHealth.isDead)
        {
            if (!_isAIActive)
            {
                ChangeAIState(isActive: true);
            }

            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
        }
        else
        {
            if (_isAIActive)
            {
                ChangeAIState(isActive: false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isHurting) return;
        currentHealth -= damage;
        PlaySound(sounds[0]);
        _animator.SetTrigger("Hurt");
        _isHurting = true;
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke(nameof(EndHurtState), 0.4f);
        }
    }

    private void EndHurtState()
    {
        _isHurting = false;
    }

    public void Die()
    {
        PlaySound(sounds[1]);
        _animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;

        ChangeAIState(isActive: false);

        enabled = false;
        Destroy(gameObject, 0.8f);
    }

    void Attack()
    {
        if (Time.time - _lastAttackTime >= attackCooldown)
        {
            _lastAttackTime = Time.time;
            _animator.SetTrigger("Attack");

            if (Vector2.Distance(transform.position, _player.position) <= attackRange)
            {
                var playerHealth = _player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }
    }

    private void PlayAttackSound()
    {
        PlaySound(sounds[2]);
    }
    
    void Flip()
    {
        if (_player == null) return;

        var direction = _player.position.x - transform.position.x;

        if (direction > 0 && !_facingRight)
        {
            _facingRight = true;
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (direction < 0 && _facingRight)
        {
            _facingRight = false;
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}