using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class Homing : MonoBehaviour
{
    public float homingRadius = 2f; 
    public float homingForce = 5f;  

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isThrown = false;
    private bool isHomingActive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab & release events
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isThrown = true;
    }

    void FixedUpdate()
    {
        if (isThrown && isHomingActive)
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
            isHomingActive = false;
        }
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
