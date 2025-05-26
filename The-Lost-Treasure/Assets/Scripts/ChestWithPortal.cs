using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestOpener : MonoBehaviour
{
    public Animator chestAnimator;
    public GameObject promptText;
    public GameObject portal;
    public GameObject fadeCanvas; // Затемнение (SpriteRenderer черного цвета с альфа = 0)

    [Header("Teleport Settings")]
    public int sceneToLoad = 1;
    public float shakeDuration = 2f;
    public float shakeMagnitude = 0.1f;
    public float fadeDuration = 2f;

    private bool isPlayerNear = false;
    private bool isChestOpened = false;
    private bool isChestDisabled = false;

    private Camera mainCamera;
    private Vector3 originalCameraPosition;

    void Start()
    {
        promptText.SetActive(false);
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isChestDisabled) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!isChestOpened)
                OpenChest();
            else
            {
                portal.SetActive(true);
                Animator portalAnimator = portal.GetComponent<Animator>();
                if (portalAnimator != null)
                {
                    portalAnimator.SetBool("on", true);
                }

                StartCoroutine(TeleportSequence());
            }
        }
    }

    void OpenChest()
    {
        isChestOpened = true;
        chestAnimator.SetBool("Open", true);
        promptText.GetComponent<TextMeshProUGUI>().text = "Нажмите Enter чтобы взять меч";
    }

    public IEnumerator TeleportSequence()
    {
        portal.SetActive(true);
        isChestOpened = false;
        isChestDisabled = true;
        chestAnimator.SetBool("Open", false);
        chestAnimator.SetBool("get", true);
        promptText.SetActive(false);

        if (mainCamera != null)
            originalCameraPosition = mainCamera.transform.position;

        StartCoroutine(ShakeCamera());
        StartCoroutine(FadeOut());

        yield return new WaitForSeconds(Mathf.Max(fadeDuration, shakeDuration));
        SceneManager.LoadScene(sceneToLoad); // Можно заменить на индекс, если нужно
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isChestDisabled)
        {
            isPlayerNear = true;
            promptText.SetActive(true);
            promptText.GetComponent<TextMeshProUGUI>().text = "Нажмите Enter чтобы открыть сундук";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isChestOpened = false;
        if (other.CompareTag("Player"))
        {
            chestAnimator.SetBool("Open", false);
            isPlayerNear = false;
            promptText.SetActive(false);
        }
    }

    IEnumerator ShakeCamera()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            if (mainCamera != null)
                mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (mainCamera != null)
            mainCamera.transform.position = originalCameraPosition;
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        SpriteRenderer sr = fadeCanvas.GetComponent<SpriteRenderer>();

        while (elapsed < fadeDuration)
        {
            float alpha = elapsed / fadeDuration;
            sr.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}