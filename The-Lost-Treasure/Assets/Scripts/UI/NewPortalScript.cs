using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public int sceneToLoad = 1; 
    public float fadeDuration = 2f;

    [Header("UI & Fade")]
    public GameObject fadeCanvas;            
    public GameObject promptTextObject;      

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
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
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
            var elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                var alpha = elapsed / fadeDuration;
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
                promptTextObject.GetComponent<TextMeshProUGUI>().text = "нажми Enter";
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