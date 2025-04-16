using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ZombieAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 2f;
    private bool isAttacking;

    public float attackRange = 2f;
    public bool roulotteInAttackRange;

    public LayerMask whatIsRoulotte;
    public int attackDamage = 10;

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null) obstacle.enabled = false; // Start disabled
    }

    private void Update()
    {
        roulotteInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsRoulotte);

        if (roulotteInAttackRange && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Stop movement and enable obstacle
        if (agent != null) agent.enabled = false;
        if (obstacle != null) obstacle.enabled = true;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, whatIsRoulotte);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.gameObject.name);
            HitEvent.GetHit(attackDamage, gameObject, hitCollider.gameObject);
        }

        yield return new WaitForSeconds(timeBetweenAttacks);

        isAttacking = false;

        // Optional: If zombie continues to attack, don't re-enable agent
        if (!roulotteInAttackRange)
        {
            if (obstacle != null) obstacle.enabled = false;
            if (agent != null) agent.enabled = true;
        }
    }


}
