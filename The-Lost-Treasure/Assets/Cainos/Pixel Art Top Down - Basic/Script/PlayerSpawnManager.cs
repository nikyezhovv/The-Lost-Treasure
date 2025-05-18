using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints; // ������ ��� ��������
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Player not found or spawnPoints not assigned.");
            return;
        }

        // ����������� ���������� ������
        GameManager.Instance.IncrementSpawnIndex(spawnPoints.Length);

        int index = GameManager.Instance.spawnIndex;
        Transform spawnPoint = spawnPoints[index];
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        Debug.Log($"[SpawnManager] ����� ������ �� ������� {index + 1} � ����� \"{scene.name}\"");
    }
}
