using UnityEngine;

public class RespawnSetter : MonoBehaviour
{
    [Tooltip("Новая точка возрождения")]
    public Vector3 newRespawnPosition;

    [Tooltip("Если true — активируется блокирующий объект (стена)")]
    public bool withBlock = false;

    [Tooltip("Объект, который станет стеной и заблокирует проход (не этот триггер)")]
    public GameObject wallToActivate;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            GameObject respawnPoint = GameObject.FindGameObjectWithTag("Respawn");

            if (respawnPoint != null)
            {
                respawnPoint.transform.position = newRespawnPosition;
                Debug.Log($"Respawn point moved to: {newRespawnPosition}");
                triggered = true;

                if (withBlock && wallToActivate != null)
                {
                    wallToActivate.SetActive(true);

                    Collider2D wallCollider = wallToActivate.GetComponent<Collider2D>();
                    if (wallCollider != null)
                    {
                        wallCollider.isTrigger = false; // Стена блокирует проход
                    }
                    else
                    {
                        Debug.LogWarning("wallToActivate не имеет Collider2D!");
                    }
                }

                // Отключим этот триггер, чтобы больше не срабатывал
                GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                Debug.LogWarning("No object with tag 'Respawn' found in the scene.");
            }
        }
    }
}