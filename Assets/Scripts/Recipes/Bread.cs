using UnityEngine;

public class Bread : MonoBehaviour
{
    [SerializeField]
    private RecipeData _recipeData;

    public RecipeData GetRecipe()
    {
        return _recipeData;
    }
}
