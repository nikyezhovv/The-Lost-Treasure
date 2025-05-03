using System;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody2D rigidBody;
    public int damage = 10;
    public float lifeTime = 2f;
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var playerColliders = player.GetComponentsInChildren<Collider2D>();
            foreach (var col in playerColliders)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col);
            }
        }

        rigidBody.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        var ghost = hitInfo.GetComponent<GhostEnemy>();
        if (ghost != null)
        {
            ghost.TakeDamage(damage);
        }
    
        var hyena = hitInfo.GetComponent<HyenaEnemy>();
        if (hyena != null)
        {
            hyena.TakeDamage(damage);
        }
    
        var golem = hitInfo.GetComponent<GolemEnemy>();
        if (golem != null)
        {
            golem.TakeDamage(damage);
        }
    
        Destroy(gameObject);
    }
}
