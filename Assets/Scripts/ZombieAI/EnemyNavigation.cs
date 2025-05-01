using UnityEngine;
using UnityEngine.AI;


public class EnemyNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    private float sideWidth;

    private Vector3 randomPoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer roulotteRenderer = LevelManager.Instance.roulote.GetComponent<Renderer>();
        if (roulotteRenderer != null)
        {
            sideWidth = roulotteRenderer.bounds.size.x; // Get world-space width
            randomPoint = GetRandomPointOnSide(LevelManager.Instance.targetZombies.position, LevelManager.Instance.targetZombies.right, sideWidth, 0f);

        }
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.SetDestination(randomPoint);

    }

    private void Update()
    {
        animator.SetBool("isWalking", true);
    }

    public Vector3 GetDestination()
    {
        return randomPoint;
    }

    public static Vector3 GetRandomPointOnSide(Vector3 center, Vector3 rightDirection, float sideWidth, float depthOffset)
    {
        float randomOffset = Random.Range(-sideWidth / 2, sideWidth / 2); // Random point along the width
        Vector3 sideCenter = center + rightDirection * depthOffset; // Move to the side of the car
        return sideCenter + rightDirection * randomOffset; // Random position along the side
    }

}
