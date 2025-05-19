using UnityEngine;

public class ChestOpener : MonoBehaviour
{
    public Animator chestAnimator;
    public GameObject swordObject; // меч внутри сундука
    public GameObject promptText;

    private bool isPlayerNear = false;
    private bool isChestOpened = false;

    void Start()
    {
        if (swordObject != null)
            swordObject.SetActive(false);

        promptText.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.KeypadEnter) && !isChestOpened)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isChestOpened = true;
        chestAnimator.SetBool("Open", true);
        if (swordObject != null)
            swordObject.SetActive(true);

        promptText.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isChestOpened)
        {
            isPlayerNear = true;
            promptText.SetActive(true);
            promptText.GetComponent<UnityEngine.UI.Text>().text = "Нажмите E чтобы открыть сундук";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            chestAnimator.SetBool("Open", false);
            isPlayerNear = false;
            promptText.SetActive(false);
        }
    }
}