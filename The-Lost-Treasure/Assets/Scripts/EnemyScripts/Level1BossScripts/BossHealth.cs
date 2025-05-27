using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : Sounds, IDamageable
{
    [SerializeField] public int maxHealth = 200;
    [SerializeField] public bool isInvulnerable;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private BossHealthBar bossHealthBar;

    [Header("Post-Death Actions")]
    [SerializeField] private GameObject[] unlockOnDeath;
    [SerializeField] private GameObject makePortal;

    [SerializeField] public int _currentHealth = 200;

    [Header("Progress Save Settings")]
    public int nextLevelValue = 1;
    private string filePath;

    [Serializable]
    public class PlayerData
    {
        public int health = 100;
        public int level = 0;
        public string weapon = "steak";
    }

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

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

            Animator anim = portalInstance.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("on", true);
            }
        }

        SaveLevelProgress(nextLevelValue);

        Destroy(gameObject);
        PlaySound(sounds[2]);
    }

    private void SaveLevelProgress(int newLevel)
    {
        PlayerData data;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            data = new PlayerData();
        }

        data.level = newLevel;

        string updatedJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, updatedJson);

        Debug.Log("+JSON файл обновлён после смерти босса: " + filePath);
    }
}