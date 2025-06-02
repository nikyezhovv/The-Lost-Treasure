using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToScene1 : MonoBehaviour
{
    public int seenNumber = 1;
    public int nextLevelNumber;
    
    private bool _isTriggered;

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
        if (_isTriggered) return;

        if (other.CompareTag("Player"))
        {
            _isTriggered = true;
            SaveLevelProgress(nextLevelNumber);
            SceneManager.LoadScene(seenNumber);
        }
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
    }
}