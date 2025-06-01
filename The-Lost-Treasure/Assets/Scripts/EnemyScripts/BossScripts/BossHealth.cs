using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : SoundEmitter, IDamageable
{
    [SerializeField] public int maxHealth = 200;
    [SerializeField] public bool isInvulnerable;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private BossHealthBar bossHealthBar;

    [Header("Post-Death Actions")]
    [SerializeField] private GameObject[] unlockOnDeath;
    [SerializeField] private GameObject makePortal;

    [SerializeField] public int currentHealth = 200;

    [Header("Progress Save Settings")]
    public int nextLevelValue = 1;
    private string _filePath;

    [Serializable]
    public class PlayerData
    {
        public int health = 100;
        public int level = 0;
        public string weapon = "steak";
    }

    private void Start()
    {
        _filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        Debug.Log($"Boss took {damage} damage. {currentHealth}HP remaining");
        currentHealth -= (int)damage;
        PlaySound(sounds[0]);
        UpdateHealthBar();

        if (currentHealth <= 200)
        {
            GetComponent<Animator>().SetBool("IsEnraged", true);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        slider.value = (float)currentHealth / maxHealth;
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
            var portalInstance = Instantiate(makePortal, transform.position, Quaternion.identity);

            var anim = portalInstance.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("on", true);
            }
        }

        SaveLevelProgress(nextLevelValue);

        Destroy(gameObject);
    }

    private void SaveLevelProgress(int newLevel)
    {
        PlayerData data;

        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);
            data = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            data = new PlayerData();
        }

        data.level = newLevel;

        var updatedJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, updatedJson);

        Debug.Log("+JSON файл обновлён после смерти босса: " + _filePath);
    }
}