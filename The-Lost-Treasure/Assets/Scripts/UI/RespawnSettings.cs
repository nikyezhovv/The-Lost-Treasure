using UnityEngine;

public class RespawnSetter : MonoBehaviour
{
    [Tooltip("����� ����� �����������")]
    public Vector3 newRespawnPosition;

    [Tooltip("���� true � ������������ ����������� ������ (�����)")]
    public bool withBlock = false;

    [Tooltip("������, ������� ������ ������ � ����������� ������ (�� ���� �������)")]
    public GameObject wallToActivate;

    private bool _triggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;

        if (other.CompareTag("Player"))
        {
            var respawnPoint = GameObject.FindGameObjectWithTag("Respawn");

            if (respawnPoint != null)
            {
                respawnPoint.transform.position = newRespawnPosition;
                Debug.Log($"Respawn point moved to: {newRespawnPosition}");
                _triggered = true;

                if (withBlock && wallToActivate != null)
                {
                    wallToActivate.SetActive(true);

                    var wallCollider = wallToActivate.GetComponent<Collider2D>();
                    if (wallCollider != null)
                    {
                        wallCollider.isTrigger = false;
                    }
                    else
                    {
                        Debug.LogWarning("wallToActivate �� ����� Collider2D!");
                    }
                }
                
                GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                Debug.LogWarning("No object with tag 'Respawn' found in the scene.");
            }
        }
    }
}