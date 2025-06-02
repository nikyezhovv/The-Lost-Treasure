using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : SoundEmitter
{
    [Header("Health Settings")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float healthRegenDelay = 2f;
    [SerializeField] public float healthRegenRate = 5f;
    [SerializeField] public Image healthBar;

    [Header("HealthBar Settings")]
    [SerializeField] public Color defaultHealthBarColor = Color.green;
    [SerializeField] public Color poisonedHealthBarColor = new(0.6f, 0, 0.7f);

    [Header("Fade & Respawn")]
    [SerializeField] public GameObject fadeCanvas;
    [SerializeField] public float fadeDuration = 0.5f;
    
    public bool isDead;
    private float _currentHealth;
    private float _timeSinceLastDamage;
    private Coroutine _poisonCoroutine;
    private PlayerControls _playerControls;
    private SpriteRenderer _fadeRenderer;
    private SpriteRenderer _playerRenderer;
    private Collider2D _playerCollider;
    

    private void Awake()
    {
        _currentHealth = maxHealth;
        _timeSinceLastDamage = 0f;
        _playerControls = GetComponent<PlayerControls>();
        _playerRenderer = GetComponentInChildren<SpriteRenderer>();
        _playerCollider = GetComponent<Collider2D>();
        if (fadeCanvas != null)
            _fadeRenderer = fadeCanvas.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isDead) return;

        if (_currentHealth < maxHealth)
        {
            _timeSinceLastDamage += Time.deltaTime;

            if (_timeSinceLastDamage >= healthRegenDelay)
                Heal(healthRegenRate * Time.deltaTime);
        }
        else
        {
            _timeSinceLastDamage = 0f;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        _playerControls.animator.SetTrigger("Hurt"); 
        _currentHealth -= damage;
        PlaySound(sounds[0]);
        _timeSinceLastDamage = 0f;
        UpdateHealthBar();

        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            PlaySound(sounds[2]);
            StartCoroutine(DieSequence());
        }
    }

    public void ApplyPoison(float damagePerTick, float duration, float tickInterval)
    {
        if (isDead) return;

        if (_poisonCoroutine != null)
        {
            PlaySound(sounds[1]);
            StopCoroutine(_poisonCoroutine);
        }

        _poisonCoroutine = StartCoroutine(PoisonCoroutine(damagePerTick, duration, tickInterval));
    }

    private IEnumerator DieSequence()
    {
        Debug.Log($"{gameObject.name} has died.");
        
        isDead = true;
        _playerControls.animator.SetTrigger("IsDead");
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        
        if (_playerControls != null)
            _playerControls.enabled = false;
        
        if (_playerCollider != null)
            _playerCollider.enabled = false;
        
        if (_fadeRenderer != null)
        {
            var elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                var alpha = elapsed / fadeDuration;
                _fadeRenderer.color = new Color(0, 0, 0, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        Respawn();
    }

    public void Respawn()
    {
        _currentHealth = maxHealth;
        UpdateHealthBar();
        UpdateHealthBarColor(defaultHealthBarColor);
        isDead = false;
        
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = true;
        
        var spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        if (spawnPoint != null)
            transform.position = spawnPoint.transform.position;
        
        if (_playerRenderer != null)
            _playerRenderer.enabled = true;
            
        if (_playerCollider != null)
            _playerCollider.enabled = true;
        
        if (_fadeRenderer != null)
            _fadeRenderer.color = new Color(0, 0, 0, 0);
        
        if (_playerControls != null)
            _playerControls.enabled = true;
    }

    private void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);
        UpdateHealthBar();
    }

    private IEnumerator PoisonCoroutine(float damagePerTick, float duration, float tickInterval)
    {
        var elapsed = 0f;
        UpdateHealthBarColor(poisonedHealthBarColor);
        _playerControls?.ApplyPoisonSlow(0.5f);

        while (elapsed < duration)
        {
            if (!isDead)
                TakeDamage(damagePerTick);

            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        _playerControls?.ClearPoisonSlow();
        UpdateHealthBarColor(defaultHealthBarColor);
        _poisonCoroutine = null;
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;
        healthBar.fillAmount = _currentHealth / maxHealth;
    }

    private void UpdateHealthBarColor(Color color)
    {
        if (healthBar == null) return;
        healthBar.color = color;
    }

    public float GetCurrentHealth() => _currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
}