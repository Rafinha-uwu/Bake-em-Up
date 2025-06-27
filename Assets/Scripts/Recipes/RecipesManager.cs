using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecipesManager : MonoBehaviour
{
	public static RecipesManager Instance { get; private set; }

	[SerializeField]
    private List<RecipeData> _recipes = new();
	[SerializeField]
	private RecipeData _badBread;

	public bool Cure = false;

    private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public bool GetCompleteRecipe(Dictionary<IngredientName, int> ingredients, out RecipeData recipe)
	{
		recipe = null;
		foreach (RecipeData item in _recipes)
		{
			if(!item.CheckIfComplete(ingredients)) continue;

			if (recipe != null && recipe.id > item.id) continue;

			if(item.name == "Cure")
			{
				if (Cure) { continue; }
				else { break; }
			}

			recipe = item;
		}

		return recipe != null;
	}

	public RecipeData GetBadBread()
	{
		return _badBread;
	}
}
