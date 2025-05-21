using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
	public float speed = 2.5f;
	public float attackRange = 3f;

	Transform player;
	Rigidbody2D rb;
	Boss boss;
	
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		rb = animator.GetComponent<Rigidbody2D>();
		boss = animator.GetComponent<Boss>();
	}
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
	    if (player == null) return;

	    float distanceToPlayer = Vector2.Distance(player.position, rb.position);

	    if (distanceToPlayer <= boss.aggroRange)
	    {
		    boss.LookAtPlayer();

		    var target = new Vector2(player.position.x, rb.position.y);
		    var newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
		    rb.MovePosition(newPos);

		    if (distanceToPlayer <= attackRange)
		    {
			    animator.SetTrigger("Attack");
		    }
	    }
    }
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("Attack");
	}
}