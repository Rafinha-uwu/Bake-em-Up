using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ZombieAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 2f;
    public bool isAttacking;

    public float attackRange = 2f;
    public bool roulotteInAttackRange;

    public LayerMask whatIsRoulotte;
    public int attackDamage = 10;

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private Animator animator;
    private Rigidbody rb;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
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
        animator.logWarnings = false;
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);
        animator.applyRootMotion = false;

        // Stop movement and enable obstacle
        if (agent != null) agent.enabled = false;
        if (obstacle != null) obstacle.enabled = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            //rb.isKinematic = true; // Only if you don't need physics anymore
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, whatIsRoulotte);
        foreach (var hitCollider in hitColliders)
        {
            HitEvent.GetHit(attackDamage, gameObject, hitCollider.gameObject);
        }

        yield return new WaitForSeconds(timeBetweenAttacks);

        isAttacking = false;
        animator.SetBool("isAttacking", false);

        if (!roulotteInAttackRange)
        {
            if (obstacle != null) obstacle.enabled = false;

            // Project zombie back onto NavMesh (in case it drifted off)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }

            if (agent != null)
            {
                agent.enabled = true;

                // Reset destination if needed
                EnemyNavigation navScript = GetComponent<EnemyNavigation>();
                if (navScript != null)
                {
                    agent.SetDestination(navScript.GetDestination());
                }
            }

            Debug.Log("Agent re-enabled. OnNavMesh: " + agent.isOnNavMesh + " Destination: " + agent.destination);
        }

    }


}
