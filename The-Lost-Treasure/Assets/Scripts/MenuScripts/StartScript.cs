using System.Collections;
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
    public GameObject fadeCanvas; // перетяни сюда затемняющий объект (SpriteRenderer с alpha = 0)
    public float fadeDuration = 2f;

    public void ChangeScenes(int numberScene)
    {
        StartCoroutine(LoadSceneWithFade(numberScene));
    }

    public void Exit()
    {
        Debug.Log("Quit called");
        Application.Quit();
    }

    private IEnumerator LoadSceneWithFade(int sceneIndex)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        // Создаём новые дефолтные данные и записываем их
        PlayerData data = new PlayerData();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("+JSON файл перезаписан: " + path);

        // Начинаем затемнение
        SpriteRenderer sr = fadeCanvas.GetComponent<SpriteRenderer>();
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = elapsed / fadeDuration;
            sr.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(0, 0, 0, 1f); // Полностью чёрный

        // Загружаем сцену
        SceneManager.LoadScene(sceneIndex);
    }
}