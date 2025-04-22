using UnityEngine;

public class GolemEnemy : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    
    [Header("Attack Settings")]
    public int damage = 10;
    public float attackCooldown = 2f;
    
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    private Transform _player;
    private Rigidbody2D _rb;
    private Animator _animator;
    private float _lastAttackTime;
    private bool _facingRight;
    private Vector2 _startingPosition;
    private bool _isReturning;
    private bool _isHurting;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _startingPosition = transform.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (_player == null || _isHurting) return;

        var distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        
        if (distanceToPlayer <= chaseRange)
        {
            _isReturning = false;
            
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            ReturnToStart();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        _animator.SetTrigger("Hurt");
        _isHurting = true;
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke("EndHurtState", 0.5f);
        }
    }

    private void EndHurtState()
    {
        _isHurting = false;
    }

    public void Die()
    {
        _animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        _rb.simulated = false;
        enabled = false;
        Destroy(gameObject, 1f);
    }

    void ChasePlayer()
    {
        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = new Vector2(direction.x * moveSpeed, _rb.linearVelocity.y);
        
        if (direction.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && _facingRight)
        {
            Flip();
        }
    }

    void ReturnToStart()
    {
        if (Vector2.Distance(transform.position, _startingPosition) < 0.1f)
        {
            _rb.linearVelocity = Vector2.zero;
            _isReturning = false;
            return;
        }
        
        _isReturning = true;
        var direction = (_startingPosition - (Vector2)transform.position).normalized;
        _rb.linearVelocity = new Vector2(direction.x * moveSpeed, _rb.linearVelocity.y);
        
        if (direction.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && _facingRight)
        {
            Flip();
        }
    }

    void Attack()
    {
        _rb.linearVelocity = Vector2.zero;
        
        if (Time.time - _lastAttackTime >= attackCooldown)
        {
            _lastAttackTime = Time.time;
            _animator.SetTrigger("Attack");
            
            if (Vector2.Distance(transform.position, _player.position) <= attackRange)
            {
                var playerHealth = _player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }

    void Flip()
    {
        _facingRight = !_facingRight;
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}