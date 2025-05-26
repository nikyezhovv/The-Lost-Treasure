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

        if (isPlayerNear && Input.GetKeyDown(KeyCode.KeypadEnter))
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
        victoryText.text = "ПОБЕДА\nПОБЕДА\nПОБЕДА\n\nВы дошли до конца!\n\nНад игрой работали:\nДмитрий\nНастя\nНикита\nТимофей";

        LayoutRebuilder.ForceRebuildLayoutImmediate(victoryText.rectTransform);
        Vector3 currentPosition = victoryText.rectTransform.localPosition;

        float initialScreenHeight = Screen.height;
        float textHeight = victoryText.preferredHeight;
        float startY = -initialScreenHeight / 2f - textHeight;
        victoryText.rectTransform.localPosition = new Vector3(currentPosition.x, startY, currentPosition.z);

        while (true)
        {
            float currentScreenHeight = Screen.height;
            LayoutRebuilder.ForceRebuildLayoutImmediate(victoryText.rectTransform);
            float currentTextHeight = victoryText.preferredHeight;

            float endY = currentScreenHeight + currentTextHeight;
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
            promptText.GetComponent<TextMeshProUGUI>().text = "Нажмите Enter чтобы открыть сундук";
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
        float elapsed = 0f;
        SpriteRenderer sr = fadeCanvas.GetComponent<SpriteRenderer>();

        while (elapsed < fadeDuration)
        {
            float alpha = elapsed / fadeDuration;
            sr.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(0, 0, 0, 1);
    }
}