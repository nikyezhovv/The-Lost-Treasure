using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRun : StateMachineBehaviour
{

    public float speed = 2.5f;
    public float attackRange = 1f;

    Transform player;
    Rigidbody2D rb;
    Golem golem;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        golem = animator.GetComponent<Golem>();

    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        golem.LookAtPlayer();

        var target = new Vector2(player.position.x, rb.position.y);
        var newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
        }
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}