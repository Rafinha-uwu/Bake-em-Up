using UnityEngine;

public class ShapedDough : MonoBehaviour
{
	[SerializeField]
	private RecipeData _recipeData;

	public RecipeData GetRecipe()
	{
		return _recipeData;
	}
}
