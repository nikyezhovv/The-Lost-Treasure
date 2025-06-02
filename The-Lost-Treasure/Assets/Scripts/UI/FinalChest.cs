using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FinalChestOpener : MonoBehaviour
{
    public Animator chestAnimator;
    public GameObject promptText;
    public GameObject portal;
    public GameObject fadeCanvas;
    public GameObject eventBubles;
    public TextMeshProUGUI victoryText;
    
    [Header("Objects to Destroy")]
    public GameObject textToDestroy;
    public GameObject healthBarToDestroy;

    [Header("Victory Settings")]
    public GameObject canvas;
    public float fadeDuration = 2f;
    public float textScrollSpeed = 100f;

    public int sceneToLoad = 0;
    public string endText;
    public string chestText = "нажми Enter чтобы открыть";

    private bool _isPlayerNear;
    private bool _isChestOpened;
    private bool _isChestDisabled;

    private void Start()
    {
        promptText.SetActive(false);
        victoryText.gameObject.SetActive(false);
        victoryText.text = "";
    }

    private void Update()
    {
        if (_isChestDisabled) return;

        if (_isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            TriggerVictoryEvent();
        }
    }

    private void TriggerVictoryEvent()
    {
        eventBubles.SetActive(true);
        _isChestDisabled = true;
        chestAnimator.SetBool("Open", true);
        promptText.SetActive(false);

        if (textToDestroy != null && healthBarToDestroy != null)
        {
            Destroy(textToDestroy);
            Destroy(healthBarToDestroy);
        }
        
        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        yield return StartCoroutine(FadeOut());
        eventBubles.SetActive(false);

        victoryText.gameObject.SetActive(true);
        victoryText.color = Color.yellow;
        victoryText.text = endText;

        LayoutRebuilder.ForceRebuildLayoutImmediate(victoryText.rectTransform);
        var currentPosition = victoryText.rectTransform.localPosition;

        var initialScreenHeight = Screen.height;
        var textHeight = victoryText.preferredHeight;
        var startY = -initialScreenHeight / 2f - textHeight;
        victoryText.rectTransform.localPosition = new Vector3(currentPosition.x, startY, currentPosition.z);

        while (true)
        {
            var currentScreenHeight = Screen.height;
            LayoutRebuilder.ForceRebuildLayoutImmediate(victoryText.rectTransform);
            var currentTextHeight = victoryText.preferredHeight;

            var endY = currentScreenHeight + currentTextHeight;
            victoryText.rectTransform.localPosition += new Vector3(0f, textScrollSpeed * Time.deltaTime, 0f);

            if (victoryText.rectTransform.localPosition.y >= endY)
                break;

            yield return null;
        }

        victoryText.gameObject.SetActive(false);
        SceneManager.LoadScene(sceneToLoad);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isChestDisabled)
        {
            _isPlayerNear = true;
            promptText.SetActive(true);
            promptText.GetComponent<TextMeshProUGUI>().text = chestText;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNear = false;
            _isChestOpened = false;
            promptText.SetActive(false);
            chestAnimator.SetBool("Open", false);
        }
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

        sr.color = new Color(0, 0, 0, 1);
    }
}