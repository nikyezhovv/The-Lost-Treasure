using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public int health = 100;
        public int level = 0;
        public string weapon = "steak";
    }

    public string fileName = "playerData.json";

    public void ChangeScenes(int numberScene)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        // Создаём новые дефолтные данные и всегда записываем их
        PlayerData data = new PlayerData();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("+JSON файл перезаписан: " + path);

        SceneManager.LoadScene(numberScene);
    }

    public void Exit()
    {
        Debug.Log("Quit called");
        Application.Quit();
    }
}
