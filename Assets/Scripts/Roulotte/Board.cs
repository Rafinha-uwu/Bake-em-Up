using UnityEngine;

public class Board : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Recipe"))
        {

            other.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        }
    }
}
