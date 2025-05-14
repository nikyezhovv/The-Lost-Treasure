using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegenDelay = 2f;
    [SerializeField] private float healthRegenRate = 5f; 
    
    public Image healthBar;
    
    private float _currentHealth;
    private float _timeSinceLastDamage;
    private bool _canRegenHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _timeSinceLastDamage = 0f;
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

    public void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        gameObject.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = _currentHealth / maxHealth;
        }
    }

    public float GetCurrentHealth() => _currentHealth;
    public float GetMaxHealth() => maxHealth;
}