using System.Collections;
using UnityEngine;

public class GolemCombat : MonoBehaviour
{
    public int attackDamage = 30;

    public float attackRange = 3f;
    public LayerMask attackMask;

    public float attackCooldown = 3f;  
    private bool canAttack = true; 

    public void Attack()
    {
        if (!canAttack)
        {
            return;
        }
        
        var pos = transform.position + new Vector3(0f, -0.5f, 0f);  // Смещаем на 0.5 единицы вниз

        
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