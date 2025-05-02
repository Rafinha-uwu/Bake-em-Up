using UnityEngine;
using UnityEngine.AI;

public class EnemyRangedAI : MonoBehaviour
{
    private Transform roulotte;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 15f;
    public float fireRate = 2f;
    public float projectileSpeed = 20f;
    private bool isAttacking;

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private float fireCooldown;

    void Start()
    {
        roulotte = LevelManager.Instance.roulote;
        agent = GetComponent<NavMeshAgent>();
        fireCooldown = 0f;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(roulotte.position, transform.position);

        if (distanceToPlayer < attackRange)
        {
            //agent.ResetPath();
            transform.LookAt(new Vector3(roulotte.position.x, transform.position.y, roulotte.position.z)); // flat look

            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
        }


        fireCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        isAttacking = true;
        if (obstacle != null) obstacle.enabled = true;
        if (agent != null) agent.enabled = false;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
        }
        isAttacking = false;


    }
}