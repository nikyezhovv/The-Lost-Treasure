using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public CanvasGroup canvasGroup;
    public Image fadeImage;
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

        PlayerData data = new PlayerData();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("+JSON был сохранён: " + path);

        float elapsed = 0f;
    
        // Сначала делаем fadeImage полностью видимым (черный экран)
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            float alpha = elapsed / fadeDuration;
            // Затемняем экран
            fadeImage.color = new Color(0, 0, 0, alpha);
            // И одновременно делаем UI менее видимым
            canvasGroup.alpha = 1 - alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // В конце делаем экран полностью черным и UI полностью невидимым
        fadeImage.color = new Color(0, 0, 0, 1f);
        canvasGroup.alpha = 0f;

        SceneManager.LoadScene(sceneIndex);
    }
}