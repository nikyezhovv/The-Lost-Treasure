using TMPro;
using UnityEngine;

public class PlotSceen : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public string text;
    public float speed = 0.01f;

    private float time;
    private int symbol = 0;
    private int playerColliderCount = 0;

    private bool IsPlayerInside => playerColliderCount > 0;

    void Start()
    {
        if (textObject != null)
        {
            textObject.text = "";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerColliderCount++;
        if (playerColliderCount == 1)
        {
            Debug.Log("Player entered");
            symbol = 0;
            time = 0f;
            if (textObject != null)
            {
                textObject.text = "";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerColliderCount = Mathf.Max(0, playerColliderCount - 1);
        if (playerColliderCount == 0)
        {
            Debug.Log("Player exited");
        }
    }

    void Update()
    {
        if (textObject == null || string.IsNullOrEmpty(text)) return;

        time += Time.deltaTime;

        if (time > speed)
        {
            time = 0f;

            if (IsPlayerInside && symbol < text.Length)
            {
                textObject.text += text[symbol];
                symbol++;
            }
            else if (!IsPlayerInside && symbol > 0)
            {
                symbol--;
                textObject.text = text.Substring(0, symbol);
            }
        }
    }
}