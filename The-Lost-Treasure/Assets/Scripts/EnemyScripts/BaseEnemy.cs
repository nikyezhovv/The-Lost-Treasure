using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseEnemy : Sounds, IDamageable
{
    [Header("Movement Settings")] [SerializeField]
    public float moveSpeed = 5f;

    [SerializeField] public float chaseRange = 6f;
    

    [Header("Attack Settings")] 
    [SerializeField] public float attackRange = 1.5f;
    [SerializeField] public int attackDamage = 7;
    [SerializeField] public float attackCooldown = 1.5f;

    [Header("Health Settings")] 
    [SerializeField] public float maxHealth = 70f;
    
    [Header("Knockback Settings")] 
    [SerializeField] public float knockbackForce = 5f;

    [Header("Jump Settings")] 
    [SerializeField] public float jumpForce = 7f;

    [SerializeField] public Transform groundCheck;
    [SerializeField] public float groundCheckRadius = 0.3f;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public float raycastOffset = 0.2f;
    [SerializeField] public float rayDistance = 1.2f;

    protected Rigidbody2D Rb;
    protected float LastAttackTime;
    protected Animator Animator;
    protected Transform Player;
    
    private Vector2 _startingPosition;
    private float _knockbackTimer;
    private float _currentHealth;
    private bool _facingRight;
    private bool _isReturning;
    private bool _isHurting;
    private bool _isKnockback;
    private bool _isGrounded;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        _startingPosition = transform.position;
        _currentHealth = maxHealth;
    }

    void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Player == null || _isHurting || _isKnockback) return;

        var distanceToPlayer = Vector2.Distance(transform.position, Player.position);

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

        if (_isKnockback)
        {
            _knockbackTimer -= Time.deltaTime;
            if (_knockbackTimer <= 0)
            {
                _isKnockback = false;
                Rb.linearVelocity = Vector2.zero;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isHurting) return;
        PlaySound(sounds[0]);
        _currentHealth -= damage;
        Animator.SetTrigger("Hurt");
        _isHurting = true;

        Vector2 knockbackDirection = (transform.position - Player.position).normalized;
        Rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke("EndHurtState", 0.4f);
        }
    }


    private void EndHurtState()
    {
        _isHurting = false;
    }

    public void Die()
    {
        PlaySound(sounds[1]);
        Animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        Rb.simulated = false;
        enabled = false;
        Destroy(gameObject, 0.8f);
    }

    void ChasePlayer()
    {
        Vector2 direction = (Player.position - transform.position).normalized;
        Rb.linearVelocity = new Vector2(direction.x * moveSpeed, Rb.linearVelocity.y);
        

        if (direction.x > 0 && !_facingRight || direction.x < 0 && _facingRight)
        {
            Flip();
        }

        CheckForObstacleAndJump(direction);
    }

    void ReturnToStart()
    {
        if (Vector2.Distance(transform.position, _startingPosition) < 1f)
        {
            Rb.linearVelocity = Vector2.zero;
            _isReturning = false;
            return;
        }

        _isReturning = true;
        var direction = (_startingPosition - (Vector2)transform.position).normalized;
        Rb.linearVelocity = new Vector2(direction.x * moveSpeed, Rb.linearVelocity.y);

        if (direction.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && _facingRight)
        {
            Flip();
        }

        CheckForObstacleAndJump(direction);
    }

    public virtual void Attack()
    {
        Rb.linearVelocity = Vector2.zero;

        if (Time.time - LastAttackTime >= attackCooldown)
        {
            LastAttackTime = Time.time;
            Animator.SetTrigger("Attack");
        }
    }
    
    private void PlayAttackSound()
    {
        //Вызывается из анимации
        PlaySound(sounds[3]);
    }
    private void DealDamage()
    {
        // Вызывается из анимации атаки
        if (Vector2.Distance(transform.position, Player.position) <= attackRange)
        {
            var playerHealth = Player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
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

    void CheckForObstacleAndJump(Vector2 direction)
    {
        if (!_isGrounded) return; 
        
        var origin = new Vector2(transform.position.x, transform.position.y - raycastOffset);
        var rayDirection = new Vector2(direction.x, 0f); 
        
        
        var hit = Physics2D.Raycast(origin, rayDirection, rayDistance, groundLayer);

        if (hit.collider != null)
        {
            Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, jumpForce);
        }
    }

    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}