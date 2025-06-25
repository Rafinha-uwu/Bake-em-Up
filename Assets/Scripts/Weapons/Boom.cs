using System.Collections;
using UnityEngine;

public class Boom : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField] private float delay = 3f;
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private GameObject explosionEffect;
    public float delayBetweenExplosions = 0.1f;
    public float explosionRadius = 2f;

    private float countdown;
    private bool hasExploded = false;
    private bool countdownStarted = false;
    private Vector3 BombLocation;

    private bool TouchedGrass = false;

    public bool cOn = true;

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
        if (!cOn) return;

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

            if (!cOn)
            {
                StartCoroutine(SpawnExplosions());
                return;
            }
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

    IEnumerator SpawnExplosions()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * explosionRadius;
            randomOffset.y = Mathf.Abs(randomOffset.y) + 1.5f; // Ensure explosions appear above ground

            Vector3 spawnPos = transform.position + randomOffset;

            Instantiate(explosionEffect, spawnPos, explosionEffect.transform.rotation);

            yield return new WaitForSeconds(delayBetweenExplosions);
        }
    }
}
