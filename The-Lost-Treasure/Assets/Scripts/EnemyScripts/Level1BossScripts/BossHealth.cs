using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] public int maxHealth = 500;
    [SerializeField] public bool isInvulnerable;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private BossHealthBar bossHealthBar;
    
    private int _currentHealth = 500;

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;

        Debug.Log($"Boss took {damage} damage. {_currentHealth}HP remaining");
        _currentHealth -= (int)damage;
        UpdateHealthBar();

        if (_currentHealth <= 200)
        {
            GetComponent<Animator>().SetBool("IsEnraged", true);
        }

        if (_currentHealth <= 0)    
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        slider.value = (float)_currentHealth / maxHealth;
    }

    public void Die()
    {
        bossHealthBar.DestroyHB();
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}