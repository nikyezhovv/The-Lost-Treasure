using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : Sounds, IDamageable
{
    [SerializeField] public int maxHealth = 500;
    [SerializeField] public bool isInvulnerable;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private BossHealthBar bossHealthBar;
    [Header("Post-Death Actions")]
    [SerializeField] private GameObject[] unlockOnDeath;
    [SerializeField] private GameObject makePortal; // ������, ������� �������� ����� ������ �����


    [SerializeField] public int _currentHealth = 500;

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;

        Debug.Log($"Boss took {damage} damage. {_currentHealth}HP remaining");
        _currentHealth -= (int)damage;
        PlaySound(sounds[0]);
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
        PlaySound(sounds[1]);
        bossHealthBar.DestroyHB();
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        foreach (var obj in unlockOnDeath)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (makePortal != null)
        {
            GameObject portalInstance = Instantiate(makePortal, transform.position, Quaternion.identity);

            // ��������� �������� ������� (���� � ���� ���� Animator)
            Animator anim = portalInstance.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("on", true);
            }
        }


        Destroy(gameObject);
        PlaySound(sounds[2]);
    }
}





//using System;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UI;

//public class BossHealth : MonoBehaviour, IDamageable
//{
//    [Header("Boss Settings")]
//    [SerializeField] public int maxHealth = 500;
//    [SerializeField] public bool isInvulnerable;

//    [Header("UI & FX")]
//    [SerializeField] private Slider slider;
//    [SerializeField] private GameObject deathEffect;
//    [SerializeField] private BossHealthBar bossHealthBar;

//    [Header("Unlock Objects On Death")]
//    [SerializeField] private GameObject[] unlockOnDeath;

//    private int _currentHealth = 500;

//    public void TakeDamage(float damage)
//    {
//        if (isInvulnerable) return;

//        Debug.Log($"Boss took {damage} damage. {_currentHealth}HP remaining");
//        _currentHealth -= (int)damage;
//        UpdateHealthBar();

//        if (_currentHealth <= 200)
//        {
//            GetComponent<Animator>().SetBool("IsEnraged", true);
//        }

//        if (_currentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    private void UpdateHealthBar()
//    {
//        slider.value = (float)_currentHealth / maxHealth;
//    }

//    public void Die()
//    {
//        bossHealthBar.DestroyHB();
//        Instantiate(deathEffect, transform.position, Quaternion.identity);

//        // ������������ ��� �������
//        foreach (var obj in unlockOnDeath)
//        {
//            if (obj != null)
//                obj.SetActive(true);
//        }

//        Destroy(gameObject);
//    }
//}