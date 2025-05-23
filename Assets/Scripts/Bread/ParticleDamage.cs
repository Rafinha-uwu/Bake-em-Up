using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField]
    protected RecipeData _recipeData;

    public RecipeData GetRecipe()
    {
        return _recipeData;
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Send");
        // Check if the collided object is the player
        if (other.gameObject.CompareTag("Zombie"))
        {
            GameObject test = other.gameObject;
            HitEvent.GetHit(_recipeData.damage, transform.gameObject, test);
        }
    }
}
