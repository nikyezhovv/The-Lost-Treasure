using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections;

public class DoorTrigers : MonoBehaviour
{
    private Animator animator;

    public int doorNumber;
    public bool isOpen;

    private void Start()
    {
        animator = GetComponent<Animator>();
        UpdateDoorVisual();
    }

    private void UpdateDoorVisual()
    {
        //Debug.Log("update" + GameManager.Instance != null + " " + GameManager.Instance.IsDoorOpen(doorNumber));
        //isOpen = GameManager.Instance != null && GameManager.Instance.IsDoorOpen(doorNumber);
        //Debug.Log("---UpdateDoorVisual " + isOpen + "  " + GameManager.Instance != null);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("enter");
        UpdateDoorVisual();
        if (isOpen)
            animator.SetBool("open", true);
            Debug.Log("Open " + animator.GetBool("open"));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        animator.SetBool("open", false);
        Debug.Log("Close " + animator.GetBool("open"));
    }
}
