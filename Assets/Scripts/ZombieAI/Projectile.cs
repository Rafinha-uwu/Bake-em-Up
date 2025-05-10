using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    public int damage = 10;
    public GameObject roulotte;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Roulotte"))
        {
            Debug.Log("ACERTOU O TIRO");
            HitEvent.GetHit(damage, gameObject, other.gameObject);
        }

        Destroy(gameObject);
    }
}