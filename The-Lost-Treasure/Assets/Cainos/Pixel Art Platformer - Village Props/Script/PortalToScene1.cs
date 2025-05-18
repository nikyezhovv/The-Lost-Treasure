using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToScene1 : MonoBehaviour
{
    private bool isTriggered = false;
    public int seenNumber = 1;
    public int nextLevelNumber = 0;

    [System.Serializable]
    public class PlayerData
    {
        public int health = 100;
        public int level = 0;
        public string weapon = "steak";
    }

    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            SaveLevelProgress(nextLevelNumber); // Устанавливаем level = 1
            SceneManager.LoadScene(seenNumber);
        }
    }

    private void SaveLevelProgress(int newLevel)
    {
        PlayerData data;

        // Если файл существует, читаем и обновляем
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            data = new PlayerData(); // создаём с дефолтными значениями
        }

        data.level = newLevel;

        string updatedJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, updatedJson);
    }
}