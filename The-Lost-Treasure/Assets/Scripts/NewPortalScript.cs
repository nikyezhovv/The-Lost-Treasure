using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public int sceneToLoad = 1; // Номер сцены, куда телепортировать
    public float fadeDuration = 2f;

    [Header("UI & Fade")]
    public GameObject fadeCanvas;              // Объект затемнения (SpriteRenderer с альфа = 0)
    public GameObject promptTextObject;        // Объект с TextMeshPro для текста "Войти"

    private bool isPlayerNear = false;
    private SpriteRenderer fadeRenderer;

    void Start()
    {
        if (promptTextObject != null)
            promptTextObject.SetActive(false);

        if (fadeCanvas != null)
            fadeRenderer = fadeCanvas.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartCoroutine(FadeAndTeleport());
        }
    }

    private IEnumerator FadeAndTeleport()
    {
        isPlayerNear = false;

        if (promptTextObject != null)
            promptTextObject.SetActive(false);

        if (fadeRenderer != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                float alpha = elapsed / fadeDuration;
                fadeRenderer.color = new Color(0, 0, 0, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (promptTextObject != null)
            {
                promptTextObject.SetActive(true);
                promptTextObject.GetComponent<TextMeshProUGUI>().text = "Нажмите Enter чтобы войти";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (promptTextObject != null)
                promptTextObject.SetActive(false);
        }
    }
}