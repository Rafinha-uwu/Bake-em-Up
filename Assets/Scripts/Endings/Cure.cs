using UnityEngine;

public class Cure : MonoBehaviour
{
    [SerializeField] private GameObject cureEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie") || other.CompareTag("Ground"))
        {
            Instantiate(cureEffect, cureEffect.transform.position, cureEffect.transform.rotation);
            GetComponentInChildren<MeshRenderer>().enabled = false;

            GetComponent<EndGame>().On = true;
        }   
        
    }
}
