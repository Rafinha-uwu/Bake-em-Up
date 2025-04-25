using UnityEngine;

public class Cream : MonoBehaviour
{
    [SerializeField] private bool Zombies = true;

    [SerializeField] private GameObject GroundCream;
    private Vector3 CreamLocation;

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
            CreamLocation.y -= 0.1f;
            Instantiate(GroundCream, CreamLocation, GroundCream.transform.rotation);

            Destroy(gameObject);
        }
    }
}
