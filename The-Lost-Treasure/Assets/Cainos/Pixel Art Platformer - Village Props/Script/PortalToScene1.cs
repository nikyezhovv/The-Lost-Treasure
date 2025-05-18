using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToScene1 : MonoBehaviour
{
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            SceneManager.LoadScene(0); // 0 Ч индекс или им€ первой сцены
        }
    }
}