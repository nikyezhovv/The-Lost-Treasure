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


    public float shakeDuration = 5f; // Длительность тряски экрана
    public float shakeMagnitude = 0.1f; // Интенсивность тряски экрана
    public float fadeDuration = 5f; // Длительность затемнения
    public GameObject canvas;

    void Start()
    {
        // Получаем ссылку на главную камеру
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found in the scene!");
            enabled = false; // Отключаем скрипт, чтобы избежать ошибок
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

        // 3. Ожидание окончания эффектов
        yield return new WaitForSeconds(Mathf.Max(shakeDuration, fadeDuration)); // Ждем окончания самого длинного эффекта
        SceneManager.LoadScene(_SceenNumber);
    }

    // Тряска камеры
    IEnumerator ShakeCamera()
    {
        var elapsed = 0.0;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0); // Трясем только по X и Y
            elapsed += Time.deltaTime;
            yield return null;
        }
    }


    //Затемнение экрана
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

