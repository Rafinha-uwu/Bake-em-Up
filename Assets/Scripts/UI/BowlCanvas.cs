using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowlCanvas : ToolCanvas
{
	[SerializeField]
	private Image _recipeImage;

	[SerializeField]
	private Sprite _emptyRecipeSprite;

	[SerializeField]
	private GameObject _ingredientCanvasElement;

	[SerializeField]
	private Transform _groupParent;

	private Dictionary<IngredientName, IngredientCanvasElement> _ingredientsInCanvas = new();

	private Transform _lookAt;

	private void Start()
	{
		_lookAt = Camera.main.transform;
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		transform.LookAt(_lookAt, Vector3.up);
		transform.Rotate(0f, 180f, 0f);
	}

	public void AddIngredient(IngredientController ingredient)
	{
		GameObject ingredientElement = Instantiate(_ingredientCanvasElement, _groupParent);

		if (ingredientElement.TryGetComponent<IngredientCanvasElement>(out var newElement))
		{
			newElement.UpdateImage(ingredient.IngredientIcon);

			_ingredientsInCanvas.Add(ingredient.IngredientName, newElement);
		}
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
		_recipeImage.sprite = _emptyRecipeSprite;
	}

	public void ChangeCanvasPosition(Transform canvasOffset)
	{
		AddTransformToFollow(canvasOffset);
	}
}
