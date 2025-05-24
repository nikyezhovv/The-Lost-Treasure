using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
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
    [SerializeField] public float fadeDuration = 2f;

    private float _currentHealth;
    private float _timeSinceLastDamage;
    private Coroutine _poisonCoroutine;
    private PlayerControls _playerControls;
    private SpriteRenderer _fadeRenderer;
    private bool isDead = false;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _timeSinceLastDamage = 0f;
        _playerControls = GetComponent<PlayerControls>();
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

        _currentHealth -= damage;
        _timeSinceLastDamage = 0f;
        UpdateHealthBar();

        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            StartCoroutine(DieSequence());
        }
    }

    public void ApplyPoison(float damagePerTick, float duration, float tickInterval)
    {
        if (isDead) return;

        if (_poisonCoroutine != null)
            StopCoroutine(_poisonCoroutine);

        _poisonCoroutine = StartCoroutine(PoisonCoroutine(damagePerTick, duration, tickInterval));
    }

    private IEnumerator DieSequence()
    {
        Debug.Log($"{gameObject.name} has died.");

        isDead = true;

        if (_playerControls != null)
            _playerControls.enabled = false;

        if (_fadeRenderer != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                float alpha = elapsed / fadeDuration;
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

        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        if (spawnPoint != null)
            transform.position = spawnPoint.transform.position;

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