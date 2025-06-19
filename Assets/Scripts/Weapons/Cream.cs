using System.Collections.Generic;
using UnityEngine;

public class Cream : MonoBehaviour
{
    [SerializeField] private bool Zombies = true;

    [SerializeField] private GameObject GroundCream;


    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponentInChildren<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
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
