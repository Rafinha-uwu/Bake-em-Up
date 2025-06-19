using UnityEngine;
using UnityEngine.AI;

public class EnemyZigzagAI : Zombie
{
    public float zigzagDistance = 3f;
    public float zigzagSpeed = 2f;
    public float movementUpdateRate = 0.2f;
    public float rotationSpeed = 5f; // Aumentado para rotação mais responsiva
    public float minDistanceToTarget = 5f; // Distância mínima para evitar oscilação

    [SerializeField] private GameObject player;
    private float timer;
    private float sideWidth;
    private Vector3 randomPoint;
    private Vector3 currentTarget;
    private Vector3 lastValidDirection;
    private ZombieAttack zombieAttack;
    private bool isNear = false;

    protected override void Start()
    {
        base.Start();
        Renderer roulotteRenderer = LevelManager.Instance.roulote.GetComponent<Renderer>();
        if (roulotteRenderer != null)
        {
            sideWidth = roulotteRenderer.bounds.size.x;
            randomPoint = EnemyNavigation.GetRandomPointOnSide(LevelManager.Instance.targetZombies.position, LevelManager.Instance.targetZombies.right, sideWidth, 0f);
        }

        timer = 0f;
        currentTarget = randomPoint;
        lastValidDirection = (randomPoint - transform.position).normalized;
        zombieAttack = GetComponent<ZombieAttack>();
        animator = GetComponent<Animator>();

        // Desabilita rotação automática do NavMeshAgent
        agent.updateRotation = false;

        // Start walk animation at random point
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(state.fullPathHash, 0, Random.Range(0f, 1f));
        animator.speed = Random.Range(0.95f, 1.05f);
    }

    void Update()
    {
        if (death || zombieAttack.isAttacking)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        timer += Time.deltaTime;

        if (timer >= movementUpdateRate && !isNear)
        {
            UpdateMovement();
            timer = 0f;
        }

        // Rotação suave baseada na direção do movimento
        HandleRotation();
    }

    private void UpdateMovement()
    {
        // Calcula direção base para o alvo
        Vector3 directionToTarget = (randomPoint - transform.position).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, directionToTarget);

        // Calcula offset do zigzag
        float zigzagOffset = Mathf.Sin(Time.time * zigzagSpeed) * zigzagDistance;
        Vector3 zigzagTarget = randomPoint + right * zigzagOffset;

        // Verifica se o novo target está longe o suficiente para evitar oscilação
        float distanceToNewTarget = Vector3.Distance(transform.position, zigzagTarget);

        if (distanceToNewTarget > minDistanceToTarget)
        {
            Debug.Log("ESTAMOS AINDA COM ZIGZAG");
            currentTarget = zigzagTarget;
            agent.SetDestination(currentTarget);

            // Atualiza última direção válida
            lastValidDirection = (currentTarget - transform.position).normalized;
            animator.SetBool("isRunning", true);
        }
        else
        {
            Debug.Log("ESTAMOS PERTO");
            currentTarget = randomPoint;
            agent.SetDestination(currentTarget);
            isNear = true;
            // Se muito próximo, continue com o target atual
            animator.SetBool("isRunning", agent.velocity.magnitude > 0.1f);
        }
    }

    private void HandleRotation()
    {
        Vector3 directionToUse;

        // Usa a velocidade do NavMeshAgent se disponível, senão usa direção calculada
        if (agent.velocity.magnitude > 0.1f)
        {
            directionToUse = agent.velocity.normalized;
        }
        else
        {
            directionToUse = lastValidDirection;
        }

        // Aplica rotação suave
        if (directionToUse != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToUse, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Método para debug - opcional
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Desenha o target atual
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTarget, 0.5f);

            // Desenha a direção do movimento
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, lastValidDirection * 2f);

            // Desenha o alvo final
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(randomPoint, 0.3f);
        }
    }
}