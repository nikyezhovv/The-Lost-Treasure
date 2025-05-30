using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalChestOpener : MonoBehaviour
{
    public Animator chestAnimator;
    public GameObject promptText;
    public GameObject portal;
    public GameObject fadeCanvas;
    public GameObject eventBubles;
    public TextMeshProUGUI victoryText;

    [Header("Victory Settings")]
    public GameObject canvas;
    public float fadeDuration = 2f;
    public float textScrollSpeed = 100f;

    public int sceneToLoad = 0;
    public string EndText;
    public string ChestText = "нажми Enter чтобы открыть";

    private bool isPlayerNear = false;
    private bool isChestOpened = false;
    private bool isChestDisabled = false;

    void Start()
    {
        promptText.SetActive(false);
        victoryText.gameObject.SetActive(false);
        victoryText.text = "";
    }

    void Update()
    {
        if (isChestDisabled) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            TriggerVictoryEvent();
        }
    }

    void TriggerVictoryEvent()
    {
        eventBubles.SetActive(true);
        isChestDisabled = true;
        chestAnimator.SetBool("Open", true);
        promptText.SetActive(false);

        StartCoroutine(VictorySequence());
    }

    IEnumerator VictorySequence()
    {
        yield return StartCoroutine(FadeOut());
        eventBubles.SetActive(false);

        victoryText.gameObject.SetActive(true);
        victoryText.color = Color.yellow;
        victoryText.text = EndText;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isChestDisabled)
        {
            isPlayerNear = true;
            promptText.SetActive(true);
            promptText.GetComponent<TextMeshProUGUI>().text = ChestText;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            isChestOpened = false;
            promptText.SetActive(false);
            chestAnimator.SetBool("Open", false);
        }
    }

    IEnumerator FadeOut()
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