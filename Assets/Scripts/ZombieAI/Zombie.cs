using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Zombie : MonoBehaviour
{
    public enum ZombieState
    {
        WALKING,
        GETHIT


    }
    [SerializeField] private int hp = 1;

    public ZombieState currentState = ZombieState.WALKING;

    private NavMeshObstacle obstacle;
    private Rigidbody[] _ragdollRigidboddies;
    private NavMeshAgent agent;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(OnDeath());
        }
    }

    public void GetHit(int damage, GameObject sender, GameObject receiver)
    {
        if (sender.CompareTag("Bread") && receiver.GetInstanceID() == gameObject.GetInstanceID())
        {
            Debug.Log("LEVASTE COM UM PAO");
            hp -= damage;
            if (hp < 1)
            {
                StartCoroutine(OnDeath());
            }
        }
    }


    IEnumerator OnDeath()
    {
        agent.ResetPath();
        yield return new WaitForSeconds(0.1f);
        EnableRagdoll();
        if (obstacle != null) obstacle.enabled = false;

        // Trigger repath for others
        ZombieRepath.RepathNearbyZombies(transform.position, 5f);

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
