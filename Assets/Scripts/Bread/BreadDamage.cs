using UnityEngine;

public class BreadDamage : MonoBehaviour
{
    [SerializeField]
    private int damage; // Amount of damage dealt to the player

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Zombie"))
        {
            
            HitEvent.GetHit(damage, transform.gameObject, collision.gameObject);
        }
    }
}
