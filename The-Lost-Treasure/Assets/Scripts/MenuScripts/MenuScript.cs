using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{ 
    public void ChangeScenes(int numberScene)
    {
        SceneManager.LoadScene(numberScene);
    }

    public void Exit()
    {
        Debug.Log("Quit called");
        Application.Quit();
    }
}
