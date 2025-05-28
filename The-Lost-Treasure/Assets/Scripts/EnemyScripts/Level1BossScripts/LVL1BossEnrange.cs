using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LVL1BossEnrange : StateMachineBehaviour
{
    public float speed = 3.5f;
    public float attackRange = 3.5f;
    
    private int _spawnCount;
    private int _spawnedCount;
    private GameObject _spawnPrefab;
    
    private Transform _player;
    private Rigidbody2D _rigidbody;
    private Boss _boss;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _rigidbody = animator.GetComponent<Rigidbody2D>();
        _boss = animator.GetComponent<Boss>();
        
        _spawnCount = _boss.spawnCount;
        _spawnPrefab = _boss.spawnPrefab;
        
        Debug.Log("Prefab: " + _spawnPrefab);

        if (_spawnedCount < _spawnCount)
        {
            Instantiate(_spawnPrefab, animator.transform.position + new Vector3(1f, 0, 0), quaternion.identity);
            _spawnedCount++;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_player == null) return;

        var distanceToPlayer = Vector2.Distance(_player.position, _rigidbody.position);

        if (distanceToPlayer <= _boss.aggroRange)
        {
            _boss.LookAtPlayer();

            var target = new Vector2(_player.position.x, _rigidbody.position.y);
            var newPos = Vector2.MoveTowards(_rigidbody.position, target, speed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(newPos);

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
