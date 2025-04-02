using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowlCanvas : ToolCanvas
{
	[SerializeField]
	private Image _recipeImage;

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

	public void UpdateRecipe(Sprite recipeSprite)
	{
		_recipeImage.sprite = recipeSprite;
	}

	public override void ClearCanvas()
	{
		ClearIngredients();
		ClearRecipe();
	}

	public void ClearIngredients()
	{
		_ingredientsInCanvas.Clear();
		foreach (Transform child in _groupParent.transform)
		{
			Destroy(child.gameObject);
		}
	}

	public void ClearRecipe()
	{
		_recipeImage.sprite = null;
	}
}
