using UnityEngine;

public class Bread : MonoBehaviour
{
    [SerializeField]
    protected RecipeData _recipeData;

	[SerializeField]
	protected bool _isBurned = false;

	public RecipeData GetRecipe()
    {
        return _recipeData;
    }

	public bool IsBurned() { return _isBurned; }

	private void OnCollisionEnter(Collision collision)
	{
		// Check if the collided object is the player
		if (collision.gameObject.CompareTag("Zombie") && !_isBurned)
		{
			GameObject test = collision.gameObject;
			HitEvent.GetHit(_recipeData.damage, transform.gameObject, test);
		}
	}
}
