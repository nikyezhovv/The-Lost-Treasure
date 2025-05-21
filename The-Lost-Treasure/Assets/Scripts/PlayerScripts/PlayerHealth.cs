using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float healthRegenDelay = 2f;
    [SerializeField] public float healthRegenRate = 5f;
    [SerializeField] public Image healthBar;

    [Header("HealthBar Settings")]
    [SerializeField] public Color defaultHealthBarColor = Color.green;
    [SerializeField] public Color poisonedHealthBarColor = new(0.6f, 0, 0.7f);
    
    private float _currentHealth;
    private float _timeSinceLastDamage;
    private bool _canRegenHealth;
    private Coroutine _poisonCoroutine;
    private PlayerControls _playerControls;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _timeSinceLastDamage = 0f;
        _playerControls = GetComponent<PlayerControls>();
    }

    private void Update()
    {
        if (_currentHealth < maxHealth)
        {
            _timeSinceLastDamage += Time.deltaTime;

            if (_timeSinceLastDamage >= healthRegenDelay)
            {
                Heal(healthRegenRate * Time.deltaTime);
            }
        }
        else
        {
            _timeSinceLastDamage = 0f;
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _timeSinceLastDamage = 0f;
        UpdateHealthBar();

        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyPoison(float damagePerTick, float duration, float tickInterval)
    {
        if (_poisonCoroutine != null)
        {
            StopCoroutine(_poisonCoroutine);
        }

        _poisonCoroutine = StartCoroutine(PoisonCoroutine(damagePerTick, duration, tickInterval));
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        gameObject.SetActive(false);
        SceneManager.LoadScene(1);
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
}