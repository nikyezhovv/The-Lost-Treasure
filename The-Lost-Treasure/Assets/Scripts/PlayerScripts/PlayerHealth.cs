using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Здесь можешь добавить логику смерти: анимацию, респавн, отключение управления и т.д.
        gameObject.SetActive(false);
    }

    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);
        Debug.Log($"{gameObject.name} healed by {amount}. Current health: {_currentHealth}");
    }

    public float GetCurrentHealth() => _currentHealth;
    public float GetMaxHealth() => maxHealth;
}