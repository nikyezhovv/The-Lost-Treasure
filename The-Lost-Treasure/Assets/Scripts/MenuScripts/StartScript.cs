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
        var path = Path.Combine(Application.persistentDataPath, fileName);

        var data = new PlayerData();
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("+JSON был сохранён: " + path);

        var elapsed = 0f;
        
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            var alpha = elapsed / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            canvasGroup.alpha = 1 - alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        fadeImage.color = new Color(0, 0, 0, 1f);
        canvasGroup.alpha = 0f;

        SceneManager.LoadScene(sceneIndex);
    }
}