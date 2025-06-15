using UnityEngine;

public class LevelEvents : MonoBehaviour
{
	public delegate void BakedNewRecipeHandler(RecipeData recipe);
	public static event BakedNewRecipeHandler OnBakedNewRecipe;

	public static void BakedNewRecipe(RecipeData recipe)
	{
		OnBakedNewRecipe?.Invoke(recipe);
	}
}
