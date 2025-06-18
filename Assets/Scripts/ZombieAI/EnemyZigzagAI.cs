using UnityEngine;
using UnityEngine.AI;

public class EnemyZigzagAI : Zombie
{
    public float zigzagDistance = 3f;
    public float zigzagSpeed = 2f;
    public float movementUpdateRate = 0.2f;
    public float rotationSpeed = 0.2f;

    [SerializeField] private GameObject player;


    private float timer;
    private float sideWidth;
    private Vector3 randomPoint;

    private ZombieAttack zombieAttack;

    protected override void Start()
    {
        base.Start();
        Renderer roulotteRenderer = LevelManager.Instance.roulote.GetComponent<Renderer>();
        if (roulotteRenderer != null)
        {
            sideWidth = roulotteRenderer.bounds.size.x; // Get world-space width
            randomPoint = EnemyNavigation.GetRandomPointOnSide(LevelManager.Instance.targetZombies.position, LevelManager.Instance.targetZombies.right, sideWidth, 0f);

        }
        timer = 0f;

        zombieAttack = GetComponent<ZombieAttack>();
        animator = GetComponent<Animator>();

        // Start walk animation at random point
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(state.fullPathHash, 0, Random.Range(0f, 1f));

        // Slight variation in animation speed
        animator.speed = Random.Range(0.95f, 1.05f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= movementUpdateRate && !death && !zombieAttack.isAttacking)
        {
            animator.SetBool("isRunning", true);
            timer = 0f;
            Vector3 directionToPlayer = (randomPoint - player.transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, directionToPlayer);

            // Zigzag offset based on time
            float zigzagOffset = Mathf.Sin(Time.time * zigzagSpeed) * zigzagDistance;

            Vector3 zigzagTarget = randomPoint + right * zigzagOffset;

            // Set destination slightly off the player to create a zigzag path
            //if(randomPoint)
            agent.SetDestination(zigzagTarget);
            //transform.forward = zigzagTarget;
            Quaternion toRotation = Quaternion.LookRotation(zigzagTarget, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}