using System.Collections;
using UnityEngine;

public class HyenaCombat : MonoBehaviour
{
    public int attackDamage = 20;

    public float attackRange = 1f;
    public LayerMask attackMask;

    public float attackCooldown = 1f;  
    private bool canAttack = true; 

    public void Attack()
    {
        if (!canAttack)
        {
            return;
        }
        
        var pos = transform.position;
        
        var colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            colInfo.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
        
        StartCoroutine(AttackCooldown());
    }
    
    private IEnumerator AttackCooldown()
    {
        canAttack = false; 
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;  
    }
    
    void OnDrawGizmosSelected()
    {
        var pos = transform.position;
        
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}