using UnityEngine;

public class Dough : MonoBehaviour
{
	[SerializeField]
	private RecipeData _recipeData;

	[SerializeField]
	private bool _isBadDough = false;

	[SerializeField]
	private bool _isShaped = false;

	public RecipeData GetRecipe()
	{
		return _recipeData;
	}
}
