using System.Collections.Generic;
using UnityEngine;

public class BowlCanvasManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _ingredientCanvasElement;

	[SerializeField]
	private Transform _groupParent;

	private Dictionary<IngredientName, IngredientCanvasElement> _ingredientsInCanvas = new();

	public void AddIngredient(IngredientController ingredient)
	{
		GameObject ingredientElement = Instantiate(_ingredientCanvasElement, _groupParent);
		if (ingredientElement.TryGetComponent<IngredientCanvasElement>(out var newElement))
		{
			newElement.UpdateImage(ingredient.IngredientIcon);

			_ingredientsInCanvas.Add(ingredient.IngredientName, newElement);
		}
	}

	public void UpdateIngredient(IngredientName name, int value)
	{
		_ingredientsInCanvas[name].UpdateCount(value);
	}
}
