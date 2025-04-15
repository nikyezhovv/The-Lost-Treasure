using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections;

public class DoorTrigers : MonoBehaviour
{
    private Animator animator;

    private bool hasKey;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        animator.SetBool("open", true);
        Debug.Log("Open " + animator.GetBool("open"));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        animator.SetBool("open", false);
        Debug.Log("Close " + animator.GetBool("open"));
    }
}
