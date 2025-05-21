using UnityEngine;

public class Boss_Enrange_Run : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float attackRange = 3f;
    
    public float ghostSpawnInterval = 20f;
    public int ghostSpawnCount = 2;

    private float ghostSpawnTimer;

    Transform player;
    Rigidbody2D rb;
    Boss boss;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
        
        ghostSpawnTimer = ghostSpawnInterval;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer();

        var target = new Vector2(player.position.x, rb.position.y);
        var newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
        }

        ghostSpawnTimer -= Time.deltaTime;
        if (ghostSpawnTimer <= 0f)
        {
            if (boss.ghostPrefab != null)
            {
                GameObject.Instantiate(boss.ghostPrefab, rb.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("ghostPrefab в Boss не назначен!");
            }

            ghostSpawnTimer = ghostSpawnInterval;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}