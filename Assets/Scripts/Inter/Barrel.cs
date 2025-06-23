using UnityEngine;

public class Barrel : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bread"))
        {
            GetComponent<Boom>().Explode();
        }

    }
}
