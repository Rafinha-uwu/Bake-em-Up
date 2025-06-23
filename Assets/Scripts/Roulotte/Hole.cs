using UnityEngine;

public class Hole : MonoBehaviour
{
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
;
        }
    }
}
