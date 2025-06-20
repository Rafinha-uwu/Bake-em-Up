using UnityEngine;

public class LevelEvents : MonoBehaviour
{
	public delegate void BakedNewRecipeHandler(RecipeData recipe);
	public static event BakedNewRecipeHandler OnBakedNewRecipe;

	public delegate void PhonesStartRingingHandler();
	public static event PhonesStartRingingHandler OnPhonesStartRinging;

	public static void BakedNewRecipe(RecipeData recipe)
	{
		OnBakedNewRecipe?.Invoke(recipe);
	}

	public static void PhonesStartRinging()
	{
		OnPhonesStartRinging?.Invoke();
	}
}
