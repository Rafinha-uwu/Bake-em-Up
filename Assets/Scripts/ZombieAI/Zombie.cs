using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

public class Zombie : MonoBehaviour
{
    public enum ZombieState
    {
        WALKING,
        GETHIT


    }
    [SerializeField] private int hp = 1;
    public UnityEvent Died;

    public ZombieState currentState = ZombieState.WALKING;

    private NavMeshObstacle obstacle;
    private Rigidbody[] _ragdollRigidboddies;
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        _ragdollRigidboddies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }

    private void OnEnable()
    {
        HitEvent.OnHit += GetHit;
    }

    private void OnDisable()
    {
        HitEvent.OnHit -= GetHit;
    }

    private void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(OnDeath());
        }
        */
    }

    public void GetHit(int damage, GameObject sender, GameObject receiver)
    {
        if (sender.CompareTag("Bread") && receiver.GetInstanceID() == gameObject.GetInstanceID())
        {
            Debug.Log("LEVASTE COM UM PAO");
            hp -= damage;

            // Get the limb hit
            Collider hitCollider = sender.GetComponent<Collider>();
            Debug.Log("Collider que acertou:" + hitCollider);
            RagdollPart hitPart = hitCollider != null ? hitCollider.GetComponent<RagdollPart>() : null;

            if (hp < 1)
            {
                Debug.Log("Parte que acertou:" + hitPart);
                StartCoroutine(OnDeath(hitPart, sender));
            }
        }
    }


    IEnumerator OnDeath(RagdollPart hitPart, GameObject sender)
    {
        Died?.Invoke();

        // Stop movement and enable obstacle
        if (agent != null) agent.enabled = true;
        if (obstacle != null) obstacle.enabled = false;
        agent.ResetPath();
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        yield return new WaitForSeconds(0.1f);
        //EnableRagdoll();
        if (obstacle != null) obstacle.enabled = false;
        if (animator != null)
            animator.enabled = false;

        // First, activate the hit limb and apply force
        if (hitPart != null)
        {
            Vector3 direction = (hitPart.transform.position - sender.transform.position).normalized;
            hitPart.AddHitForce(direction * 50f);
        }

        yield return new WaitForSeconds(0.1f);


        foreach (RagdollPart part in GetComponentsInChildren<RagdollPart>())
        {
            Debug.Log("Acertou:" + part);
            part.Activate();
        }

        // Trigger repath for others
        ZombieRepath.RepathNearbyZombies(transform.position, 5f, gameObject);

        yield return new WaitForSeconds(5f);
        // Destroy the zombie
        Destroy(gameObject);
    }

    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidboddies)
        {
            rigidbody.isKinematic = true;
        }
    }

    private void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidboddies)
        {
            rigidbody.isKinematic = false;
        }
    }
}
