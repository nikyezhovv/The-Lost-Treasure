using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PortalToLevel : MonoBehaviour
{
    public int _SceenNumber;
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private bool isTriggered = false;


    public float shakeDuration = 5f; // ������������ ������ ������
    public float shakeMagnitude = 0.1f; // ������������� ������ ������
    public float fadeDuration = 5f; // ������������ ����������
    public GameObject canvas;

    void Start()
    {
        // �������� ������ �� ������� ������
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found in the scene!");
            enabled = false; // ��������� ������, ����� �������� ������
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("teleport to " + _SceenNumber);
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            originalCameraPosition = mainCamera.transform.position;
            StartCoroutine(TriggerSequence());
        }
    }

    IEnumerator TriggerSequence()
    {
        StartCoroutine(FadeOut());
        StartCoroutine(ShakeCamera());

        // 3. �������� ��������� ��������
        yield return new WaitForSeconds(Mathf.Max(shakeDuration, fadeDuration)); // ���� ��������� ������ �������� �������
        SceneManager.LoadScene(_SceenNumber);
    }

    // ������ ������
    IEnumerator ShakeCamera()
    {
        var elapsed = 0.0;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0); // ������ ������ �� X � Y
            elapsed += Time.deltaTime;
            yield return null;
        }
    }


    //���������� ������
    IEnumerator FadeOut()
    {

        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            canvas.GetComponent<SpriteRenderer>().color =
                new Color(0, 0, 0, (elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

