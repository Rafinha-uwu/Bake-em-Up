using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Scriptable Objects/RecipeData")]
public class RecipeData : ScriptableObject
{
    public int id;
    public List<IngredientName> ingredients = new();
    public Sprite recipeSprite;

	public bool CheckIfComplete(Dictionary<IngredientName, int> ingredientsInsideBowl)
    {
        foreach(IngredientName ingrediant in ingredients)
        {
            if (!ingredientsInsideBowl.ContainsKey(ingrediant)) return false;
        }

        return true;
    }
}
