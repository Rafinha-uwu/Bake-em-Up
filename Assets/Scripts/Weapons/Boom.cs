using UnityEngine;

public class Boom : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField] private float delay = 3f;
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private GameObject explosionEffect;

    private float countdown;
    private bool hasExploded = false;
    private bool countdownStarted = false;
    private Vector3 BombLocation;

    private bool TouchedGrass = false;

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        if (countdownStarted && !hasExploded)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                BombLocation = transform.position;
                BombLocation.y += 1.5f;
                Explode();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        if (collision.gameObject.CompareTag("Zombie"))
        {
            BombLocation = transform.position;
            if (TouchedGrass) { BombLocation.y += 1.5f; }
            Explode(); // explode instantly
        }
        else if (collision.gameObject.CompareTag("Ground") && !countdownStarted)
        {
            TouchedGrass = true;
            countdownStarted = true;
            gameObject.GetComponent<Animator>().Play("Boom");
        }
    }

    public void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, BombLocation, explosionEffect.transform.rotation);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }

            if (nearbyObject.gameObject.CompareTag("Zombie"))
            {
                HitEvent.GetHit(damage, gameObject, nearbyObject.gameObject);
            }
        }

        Destroy(gameObject);
    }
}
