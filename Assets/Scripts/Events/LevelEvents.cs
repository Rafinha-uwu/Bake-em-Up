using UnityEngine;

public class LevelEvents : MonoBehaviour
{
	public delegate void BakedNewRecipeHandler(RecipeData recipe);
	public static event BakedNewRecipeHandler OnBakedNewRecipe;

	public delegate void PhoneStartRingingHandler(string nodeName);
	public static event PhoneStartRingingHandler OnPhoneStartRinging;

	public static void BakedNewRecipe(RecipeData recipe)
	{
		OnBakedNewRecipe?.Invoke(recipe);
	}

	public static void PhoneStartRinging(string nodeName)
	{
		OnPhoneStartRinging?.Invoke(nodeName);
	}
}
