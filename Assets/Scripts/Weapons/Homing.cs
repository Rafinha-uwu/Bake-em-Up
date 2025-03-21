using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class Homing : MonoBehaviour
{
    public float homingRadius = 2f;  // Detection range for zombies
    public float homingForce = 5f;   // Strength of homing effect
    public float minDistanceFromPlayer = 1f; // How far the bread must be from the player before homing starts

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isThrown = false;
    private bool isHomingActive = true;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Find the player (assuming they have the "Player" tag)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj)
        {
            player = playerObj.transform;
        }

        // Subscribe to grab & release events
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isThrown = true; // Mark as thrown
    }

    void FixedUpdate()
    {
        if (isThrown && isHomingActive && IsFarEnoughFromPlayer())
        {
            GameObject closestZombie = FindClosestZombie();
            if (closestZombie)
            {
                Vector3 direction = (closestZombie.transform.position - transform.position).normalized;
                rb.AddForce(direction * homingForce, ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Zombie") || collision.gameObject.CompareTag("Ground"))
        {
            isHomingActive = false; // Stop homing on impact
        }
    }

    private bool IsFarEnoughFromPlayer()
    {
        if (player == null) return true; // If no player found, always allow homing
        return Vector3.Distance(transform.position, player.position) > minDistanceFromPlayer;
    }

    GameObject FindClosestZombie()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        GameObject closest = null;
        float closestDistance = homingRadius;

        foreach (GameObject zombie in zombies)
        {
            float distance = Vector3.Distance(transform.position, zombie.transform.position);
            if (distance < closestDistance)
            {
                closest = zombie;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}
