using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class DoorTrigers : MonoBehaviour
{
    private Animator animator;

    public int doorNumber;
    public bool isOpen;
    private Light doorLight; // Если ты используешь Light2D


    private void Start()
    {
        animator = GetComponent<Animator>();
        doorLight = GetComponentInChildren<Light>(); // Найдёт свет внутри объекта

        UpdateDoorVisual();
    }

    private void UpdateDoorVisual()
    {
        if (doorLight != null)
        {
            doorLight.enabled = isOpen;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UpdateDoorVisual();
        if (isOpen)
            animator.SetBool("open", true);
        Debug.Log("+enter isOpen " + isOpen + " " + animator.GetBool("open"));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        animator.SetBool("open", false);
        Debug.Log("Close " + animator.GetBool("open"));
    }
}
