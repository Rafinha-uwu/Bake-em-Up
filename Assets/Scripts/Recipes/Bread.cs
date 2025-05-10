using UnityEngine;

public class Bread : MonoBehaviour
{
    [SerializeField]
    private RecipeData _recipeData;

	public RecipeData GetRecipe()
    {
        return _recipeData;
    }

	private void OnCollisionEnter(Collision collision)
	{
		// Check if the collided object is the player
		if (collision.gameObject.CompareTag("Zombie"))
		{
			HitEvent.GetHit(_recipeData.damage, transform.gameObject, collision.gameObject);
		}
	}
}
