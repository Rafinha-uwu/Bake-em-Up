using UnityEngine;

public class Donut : Bread
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zombie") && !_isBurned)
        {
            HitEvent.GetHit(_recipeData.damage, transform.gameObject, other.gameObject);
        }
    }
}
