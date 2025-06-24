using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

public class Zombie : MonoBehaviour
{
    public enum ZombieState
    {
        WALKING,
        GETHIT,
        ATTACK


    }
    [SerializeField] private int hp = 1;
    public UnityEvent Died;
    public bool death = false;
    public float force = 50f;

    public ZombieState currentState = ZombieState.WALKING;

    private NavMeshObstacle obstacle;
    private Rigidbody[] _ragdollRigidboddies;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected AudioSource _audioSource;
    [SerializeField] private AudioClip zombie_scream;
    [SerializeField] private AudioClip hit_sound;

    [SerializeField]
    private GameObject HIT;

    [SerializeField]
    private GameObject EP;

    [SerializeField]
    private GameObject POW;

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

    protected virtual void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        obstacle = GetComponent<NavMeshObstacle>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(currentState == ZombieState.WALKING)
        {
            if (!_audioSource.isPlaying)
            {
                PlayWalkSound();
            }
        }
    }

    public void GetHit(int damage, GameObject sender, GameObject receiver)
    {
        if (sender.CompareTag("Bread") && receiver.transform.IsChildOf(transform))
        {
            currentState = ZombieState.GETHIT;
            //Debug.Log("LEVASTE COM UM PAO");
            hp -= damage;

            PlayHitSound();

            // Get the limb hit
            Collider hitCollider = receiver.GetComponent<Collider>();
            //Debug.Log("Collider que acertou:" + hitCollider.name);
            RagdollPart hitPart = hitCollider != null ? hitCollider.GetComponent<RagdollPart>() : null;
            // Array of your object options
            GameObject[] options = new GameObject[] { HIT, EP, POW };

            // Choose one at random
            int index = Random.Range(0, options.Length);
            GameObject chosenPrefab = options[index];

            // Instantiate it at hitPart's position
            Instantiate(chosenPrefab, hitPart.transform.position, Quaternion.identity);

            if (hp < 1 && !death)
            {
                //Debug.Log("Parte que acertou:" + hitPart.transform.name);
                StartCoroutine(OnDeath(hitPart, sender.transform.position, receiver));
            }
        }
    }

    IEnumerator OnDeath(RagdollPart hitPart, Vector3 senderPosition, GameObject receiver)
    {
        Debug.Log("Agente: " + agent.isActiveAndEnabled);
        agent.ResetPath();
        Died?.Invoke();
        death = true;
        
        // Stop movement and enable obstacle
        if (agent != null) agent.enabled = false;
        if (obstacle != null) obstacle.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        yield return new WaitForSeconds(0.1f);
        //EnableRagdoll();
        if (obstacle != null) obstacle.enabled = false;
        if (animator != null)
            animator.enabled = false;

        // First, activate the hit limb and apply force
        if (hitPart != null)
        {
            Vector3 direction = (hitPart.transform.position - senderPosition).normalized;
            hitPart.AddHitForce(direction * force);
        }

        yield return new WaitForSeconds(0.05f);


        foreach (RagdollPart part in GetComponentsInChildren<RagdollPart>())
        {
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

    private void PlayWalkSound()
    {
        _audioSource.PlayOneShot(zombie_scream);
    }

    private void PlayHitSound()
    {
        _audioSource.clip = hit_sound;
        _audioSource.Play();
    }

    void SetTagInChildren(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.tag = tag;
            SetTagInChildren(child.gameObject, tag);
        }
    }
}
