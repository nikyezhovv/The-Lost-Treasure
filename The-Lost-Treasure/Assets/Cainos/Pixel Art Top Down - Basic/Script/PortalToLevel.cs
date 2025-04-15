using UnityEngine;
using UnityEngine.SceneManagement;


public class PortalToLevel : MonoBehaviour
{
    public int SceenNumber;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("teleport to " + SceenNumber);
        SceneManager.LoadScene(SceenNumber);
    }
}
