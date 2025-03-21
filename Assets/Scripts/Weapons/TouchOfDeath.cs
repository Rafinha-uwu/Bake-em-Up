using UnityEngine;

public class TouchOfDeath : MonoBehaviour
{
    public float destroyDelay = 2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Zombie") || collision.gameObject.CompareTag("Ground"))
        {
            Invoke(nameof(DestroySelf), destroyDelay);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
