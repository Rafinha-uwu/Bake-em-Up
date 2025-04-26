using UnityEngine;

public class PastryCream : MonoBehaviour
{
	[SerializeField]
	private RecipeData _recipeData;

	private Color _color; //USAR DEPOIS

	public RecipeData GetRecipe()
	{
		return _recipeData;
	}

	//Usar Depois
	public void SetColor(Color color)
	{
		//A cor recebida vai ter sido o resultado obtido de um script "ColorCream" que irá passar o resultante para aqui
		_color = color;
	}
}
