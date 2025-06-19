using System.Collections.Generic;
using UnityEngine;

public class Cream : MonoBehaviour
{
    [SerializeField] private bool Zombies = true;

    [SerializeField] private GameObject GroundCream;
        private Vector3 CreamLocation;


    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponentInChildren<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie") && Zombies)
        {
            CreamLocation = other.transform.position;
            CreamLocation.y -= 1.1f;
            Instantiate(GroundCream, CreamLocation, GroundCream.transform.rotation);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            CreamLocation = transform.position;
            CreamLocation.y -= 0f;
            Instantiate(GroundCream, CreamLocation, GroundCream.transform.rotation);

            Destroy(gameObject);
        }
    }

    public void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        if (numCollisionEvents > 0)
        {
            Vector3 collisionPos = collisionEvents[0].intersection;

            if (other.CompareTag("Zombie") && Zombies)
            {
                collisionPos.y -= 1.1f;
                Instantiate(GroundCream, collisionPos, GroundCream.transform.rotation);
            }
            else if (other.CompareTag("Ground"))
            {
                Instantiate(GroundCream, collisionPos, GroundCream.transform.rotation);
            }

            Destroy(gameObject.transform.parent.gameObject);
        }
    }

}
