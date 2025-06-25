using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField]
    protected int damage;

    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            GameObject test = other.gameObject;
            HitEvent.GetHit(damage, transform.gameObject, test);
        }
    }
}
