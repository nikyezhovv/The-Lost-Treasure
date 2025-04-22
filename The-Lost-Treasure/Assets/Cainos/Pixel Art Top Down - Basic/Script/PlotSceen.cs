using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlotSceen : MonoBehaviour
{
    public Text textObject;
    public bool isEnter = false;
    public string text;
    private double time;
    private int symbol = 0;

    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        textObject.text = "";
        Debug.Log("we enter");
        isEnter = true;
        symbol = 0;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        isEnter = false;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.1)
        {
            time = 0;
            if (isEnter && symbol < text.Length)
            {
                textObject.text += text[symbol];
                symbol += 1;
            }

            if (!isEnter)
            {
                if (symbol >= 0)
                {
                    textObject.text = textObject.text.Substring(0, symbol);
                    symbol -= 1;
                }
            }
        }
    }
}
