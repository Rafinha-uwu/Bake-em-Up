using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRangedAI : Zombie
{
    private Transform roulotte;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int attackDamage = 5;
    public float attackRange = 15f;
    public float fireRate = 2f;
    public float projectileSpeed = 20f;
    private bool isAttacking;

    private float fireCooldown;

    [SerializeField] private AudioClip throw_sound;

    protected override void Start()
    {
        base.Start();
        roulotte = LevelManager.Instance.roulote;
        //agent = GetComponent<NavMeshAgent>();
        fireCooldown = 0f;
        //animator = GetComponent<Animator>();
        //_audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(roulotte.position, transform.position);

        if (distanceToPlayer < attackRange)
        {
            //agent.ResetPath();
            transform.LookAt(new Vector3(roulotte.position.x, transform.position.y, roulotte.position.z)); // flat look

            if (fireCooldown <= 0f && !death)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
                HitEvent.GetHit(attackDamage, gameObject, roulotte.gameObject);
                fireCooldown = 1f / fireRate;
            }
        }


        fireCooldown -= Time.deltaTime;
    }

    private IEnumerator Shoot()
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
        PlayThrowSound();
        yield return new WaitForSeconds(2.5f);
        animator.SetBool("isAttacking", false);
        isAttacking = false;


    }

    private void PlayThrowSound()
    {
        _audioSource.clip = throw_sound;
        _audioSource.Play();
    }
}