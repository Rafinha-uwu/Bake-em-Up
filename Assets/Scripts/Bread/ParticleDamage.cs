using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField]
    protected int damage;

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Col");
        if (other.gameObject.CompareTag("Zombie"))
        {
            Debug.Log("Hit");
            GameObject test = other.gameObject;
            HitEvent.GetHit(damage, transform.gameObject, test);
        }
    }
}
