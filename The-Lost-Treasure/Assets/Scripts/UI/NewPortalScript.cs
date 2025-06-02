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

    private bool _isPlayerNear;
    private SpriteRenderer _fadeRenderer;

    private void Start()
    {
        if (promptTextObject != null)
            promptTextObject.SetActive(false);

        if (fadeCanvas != null)
            _fadeRenderer = fadeCanvas.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(FadeAndTeleport());
        }
    }

    private IEnumerator FadeAndTeleport()
    {
        _isPlayerNear = false;

        if (promptTextObject != null)
            promptTextObject.SetActive(false);

        if (_fadeRenderer != null)
        {
            var elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                var alpha = elapsed / fadeDuration;
                _fadeRenderer.color = new Color(0, 0, 0, alpha);
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
            _isPlayerNear = true;
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
            _isPlayerNear = false;
            if (promptTextObject != null)
                promptTextObject.SetActive(false);
        }
    }
}