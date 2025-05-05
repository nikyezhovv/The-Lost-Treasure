using System;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody2D rigidBody;
    public int damage = 20;
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
        var damageable = hitInfo.GetComponent<IDamageable>();
        
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
