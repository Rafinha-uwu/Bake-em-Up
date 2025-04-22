using UnityEngine;
using UnityEngine.AI;

public class EnemyZigzagAI : MonoBehaviour
{
    public float zigzagDistance = 3f;
    public float zigzagSpeed = 2f;
    public float movementUpdateRate = 0.2f;

    private NavMeshAgent agent;
    private float timer;
    private float sideWidth;
    private Vector3 randomPoint;

    void Start()
    {
        Renderer roulotteRenderer = LevelManager.Instance.roulote.GetComponent<Renderer>();
        if (roulotteRenderer != null)
        {
            sideWidth = roulotteRenderer.bounds.size.x; // Get world-space width
            randomPoint = EnemyNavigation.GetRandomPointOnSide(LevelManager.Instance.targetZombies.position, LevelManager.Instance.targetZombies.right, sideWidth, 0f);

        }
        agent = GetComponent<NavMeshAgent>();
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= movementUpdateRate)
        {
            timer = 0f;
            Vector3 directionToPlayer = (randomPoint - transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, directionToPlayer);

            // Zigzag offset based on time
            float zigzagOffset = Mathf.Sin(Time.time * zigzagSpeed) * zigzagDistance;

            Vector3 zigzagTarget = randomPoint + right * zigzagOffset;

            // Set destination slightly off the player to create a zigzag path
            agent.SetDestination(zigzagTarget);
        }
    }
}