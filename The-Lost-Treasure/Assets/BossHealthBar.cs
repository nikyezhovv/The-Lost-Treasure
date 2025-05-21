using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] public Transform boss;
    [SerializeField] public Vector3 offset = new Vector3(0, 0.1f, 0);

    private void Update()
    {
        if (boss != null)
        {
            // Обновляем позицию, но не поворот
            transform.position = boss.position + offset;
            transform.rotation = Quaternion.identity; // или Camera.main.transform.rotation, если нужно billboard
        }
    }

    public void DestroyHB()
    {
        Destroy(gameObject);
    }
}