using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestOpener : MonoBehaviour
{
    public Animator chestAnimator;
    public GameObject promptText;
    public GameObject portal;
    public GameObject fadeCanvas;
    public string TextToOpen;
    public string TextToTouch;

    [Header("Teleport Settings")]
    public int sceneToLoad = 1;
    public float shakeDuration = 2f;
    public float shakeMagnitude = 0.1f;
    public float fadeDuration = 2f;
    

    private bool _isPlayerNear = false;
    private bool _isChestOpened = false;
    private bool _isChestDisabled = false;

    private Camera _mainCamera;
    private Vector3 _originalCameraPosition;

    private void Start()
    {
        promptText.SetActive(false);
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_isChestDisabled) return;

        if (_isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            if (!_isChestOpened)
                OpenChest();
            else
            {
                portal.SetActive(true);
                var portalAnimator = portal.GetComponent<Animator>();
                if (portalAnimator != null)
                {
                    portalAnimator.SetBool("on", true);
                }

                StartCoroutine(TeleportSequence());
            }
        }
    }

    private void OpenChest()
    {
        _isChestOpened = true;
        chestAnimator.SetBool("Open", true);
        promptText.GetComponent<TextMeshProUGUI>().text = TextToOpen;
    }

    public IEnumerator TeleportSequence()
    {
        portal.SetActive(true);
        _isChestOpened = false;
        _isChestDisabled = true;
        chestAnimator.SetBool("Open", false);
        chestAnimator.SetBool("get", true);
        promptText.SetActive(false);

        if (_mainCamera != null)
            _originalCameraPosition = _mainCamera.transform.position;

        StartCoroutine(ShakeCamera());
        StartCoroutine(FadeOut());

        yield return new WaitForSeconds(Mathf.Max(fadeDuration, shakeDuration));
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isChestDisabled)
        {
            _isPlayerNear = true;
            promptText.SetActive(true);
            promptText.GetComponent<TextMeshProUGUI>().text = TextToTouch;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isChestOpened = false;
        if (other.CompareTag("Player"))
        {
            chestAnimator.SetBool("Open", false);
            _isPlayerNear = false;
            promptText.SetActive(false);
        }
    }

    private IEnumerator ShakeCamera()
    {
        var elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            var x = Random.Range(-1f, 1f) * shakeMagnitude;
            var y = Random.Range(-1f, 1f) * shakeMagnitude;

            if (_mainCamera != null)
                _mainCamera.transform.position = _originalCameraPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_mainCamera != null)
            _mainCamera.transform.position = _originalCameraPosition;
    }

    private IEnumerator FadeOut()
    {
        var elapsed = 0f;
        var sr = fadeCanvas.GetComponent<SpriteRenderer>();

        while (elapsed < fadeDuration)
        {
            var alpha = elapsed / fadeDuration;
            sr.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}