using UnityEngine;

public class Donut : Bread
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is the player
        if (other.gameObject.CompareTag("Zombie") && !_isBurned)
        {
            GameObject test = other.gameObject;
            HitEvent.GetHit(_recipeData.damage, transform.gameObject, test);
        }
    }
}
