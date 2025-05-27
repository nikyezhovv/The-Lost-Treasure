using TMPro;
using UnityEngine;

public class PlotSceen : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public bool isEnter = false;
    public string text;
    private float time;
    private int symbol = 0;

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

        Debug.Log("we enter");
        isEnter = true;
        symbol = 0;
        time = 0f;

        if (textObject != null)
        {
            textObject.text = "";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isEnter = false;
    }

    void Update()
    {
        if (textObject == null || string.IsNullOrEmpty(text)) return;

        time += Time.deltaTime;

        if (time > 0.05f)
        {
            time = 0;

            if (isEnter && symbol < text.Length)
            {
                textObject.text += text[symbol];
                symbol += 1;
            }
            else if (!isEnter && symbol >= 0)
            {
                symbol -= 1;
                textObject.text = text.Substring(0, Mathf.Max(symbol, 0));
            }
        }
    }
}