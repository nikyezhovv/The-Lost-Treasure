using UnityEngine;

public class GhostEnemy : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float chaseRange = 6f;
    public float attackRange = 1.8f;
    public float hoverHeight = 1f;
    
    [Header("Attack Settings")]
    public int damage = 3;
    public float attackCooldown = 2.5f;
    public float fadeBeforeAttack = 0.3f;
    
    [Header("Health Settings")]
    public float maxHealth = 80f;
    public float currentHealth = 20f;
    
    private Transform _player;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private float _lastAttackTime;
    private bool _facingRight;
    private Vector2 _startingPosition;
    private bool _isReturning;
    private bool _isHurting;
    private Color _originalColor;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startingPosition = transform.position;
        currentHealth = maxHealth;
        _originalColor = _spriteRenderer.color;
        
        _startingPosition.y += hoverHeight;
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

        // Отталкивание от игрока
        Vector2 knockbackDirection = (transform.position - _player.position).normalized;
        float knockbackForce = 5f; // сила отталкивания (можно вынести в публичное поле)
        _rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke("EndHurtState", 0.4f); // или 0.5f для голема
        }
    }


    private void EndHurtState()
    {
        _isHurting = false;
        _spriteRenderer.color = _originalColor;
    }

    public void Die()
    {
        _animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        _rb.simulated = false;
        enabled = false;
        
        StartCoroutine(FadeOut(1f));
    }

    System.Collections.IEnumerator FadeOut(float duration)
    {
        var timer = 0f;
        var startColor = _spriteRenderer.color;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            var alpha = Mathf.Lerp(1f, 0f, timer / duration);
            _spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        
        Destroy(gameObject);
    }

    void ChasePlayer()
    {
        var targetPosition = new Vector2(_player.position.x, _player.position.y + hoverHeight);
        var direction = (targetPosition - (Vector2)transform.position).normalized;
        _rb.linearVelocity = direction * moveSpeed;
        
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
        _rb.linearVelocity = direction * moveSpeed;
        
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
            
            StartCoroutine(FadeBeforeAttack());
            
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

    System.Collections.IEnumerator FadeBeforeAttack()
    {
        var semiTransparent = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0.5f);
        _spriteRenderer.color = semiTransparent;
        yield return new WaitForSeconds(fadeBeforeAttack);
        _spriteRenderer.color = _originalColor;
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
        
        Gizmos.color = Color.cyan;
        var hoverPos = transform.position;
        hoverPos.y += hoverHeight;
        Gizmos.DrawWireSphere(hoverPos, 0.2f);
    }
}