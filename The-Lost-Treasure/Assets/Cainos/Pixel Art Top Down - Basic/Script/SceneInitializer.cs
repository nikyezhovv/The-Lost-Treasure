using UnityEngine;
using System.IO;

public class SceneInitializer : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public int health;
        public int level;
        public string weapon;
    }

    public DoorTrigers door1;
    public DoorTrigers door2;
    public DoorTrigers door3;

    public Transform spawnPoint0;
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public string fileName = "playerData.json";

    void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log(path);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            ApplyDoorLogic(data.level);
            SpawnPlayer(data.level);
        }
        else
        {
            Debug.LogWarning("+Файл playerData.json не найден!");
        }
    }

    private void ApplyDoorLogic(int level)
    {
        door1.isOpen = (level == 0);
        door2.isOpen = (level == 1);
        door3.isOpen = (level == 2);
    }

    private void SpawnPlayer(int level)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Игрок с тегом 'Player' не найден!");
            return;
        }

        switch (level)
        {
            case 0:
                player.transform.position = spawnPoint0.position;
                break;
            case 1:
                player.transform.position = spawnPoint1.position;
                break;
            case 2:
                player.transform.position = spawnPoint2.position;
                break;
            default:
                Debug.LogWarning("Неизвестный уровень: " + level);
                break;
        }

        string[] layerNames = { "Layer 1", "Layer 2", "Layer 2" };
        if (level >= 0 && level < layerNames.Length)
        {
            int layerIndex = LayerMask.NameToLayer(layerNames[level]);
            if (layerIndex != -1)
            {
                player.layer = layerIndex;
            }
            else
            {
                Debug.LogWarning($"Layer '{layerNames[level]}' не найден в проекте.");
            }
        }

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = level == 0 ? 2 : 23; // Пример: Level0 = 0, Level1 = 10, Level2 = 20
        }
        else
        {
            Debug.LogWarning("У игрока нет SpriteRenderer!");
        }
    }
}