using UnityEngine;

public class RecipeBoardTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Recipe"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                other.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                rb.isKinematic = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Recipe"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }
}
