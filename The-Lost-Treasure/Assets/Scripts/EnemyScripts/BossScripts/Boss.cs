using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] public float aggroRange = 40f;
    [SerializeField] public Transform player;
    [SerializeField] public int spawnCount = 3;
    [SerializeField] public GameObject spawnPrefab;
    public bool isFlipped;

    public void LookAtPlayer()
    {
        var flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

}