using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class Bowl : ToolContainer
{
	[SerializeField]
	private GameObject _container;

	private BowlCanvas _bowlCanvas;
	private RecipeData _recipeData;

	private Dictionary<IngredientName, int> _ingredientsInside = new();

	[HideInInspector]
	public bool HasCompletedDough = false;
	[HideInInspector]
	public bool HasBadDough = false;
	private int _doughQuantity = 0;

	protected override void Awake()
	{
		base.Awake();
		_bowlCanvas = _toolCanvas.gameObject.GetComponent<BowlCanvas>();
	}

	public bool GetRecipe(out RecipeData recipe)
	{
		recipe = _recipeData;

		return recipe != null;
	}

	public void MakeDough()
	{
		ClearBowl();

		HasCompletedDough = true;

		GameObject firstDough = Instantiate(_recipeData.doughPrefab);
		InsertItem(firstDough);
		GameObject secondDough = Instantiate(_recipeData.doughPrefab);
		InsertItem(secondDough);
		_doughQuantity = 2;
	}

	public void MakeBadDough()
	{
		ClearBowl();

		HasBadDough = true;

		GameObject badDough = Instantiate(RecipesManager.Instance.GetBadDough());
		InsertItem(badDough);
		_doughQuantity = 1;
	}

	public void ClearBowl()
	{
		HasCompletedDough = false;
		HasBadDough = false;
		_doughQuantity = 0;

		_ingredientsInside.Clear();
		foreach (Transform child in _container.transform)
		{
			Destroy(child.gameObject);
		}
		_bowlCanvas.ClearIngredients();
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject obj = other.gameObject;

		if (obj.TryGetComponent<XRGrabInteractable>(out var interactable))
		{
			if (!interactable.isSelected) return;
			ReleaseItem(interactable);
			InsertItem(obj);
		}
	}

	private void InsertItem(GameObject obj)
	{
		obj.transform.SetParent(_container.transform, true);
		obj.transform.localPosition = Vector3.zero;
		int LayerInsideBowl = LayerMask.NameToLayer("Inside Bowl");
		obj.layer = LayerInsideBowl;

		if(obj.TryGetComponent<IngredientController>(out var ingredient))
		{
			AddIngredient(ingredient);
		}
	}

	private void AddIngredient(IngredientController ingredient)
	{
		IngredientName name = ingredient.IngredientName;
		if(_ingredientsInside.TryGetValue(name, out int value))
		{
			_ingredientsInside[name] += 1;
			_bowlCanvas.UpdateIngredient(name, _ingredientsInside[name]);
		}
		else
		{
			_ingredientsInside.Add(name, 1);
			_bowlCanvas.AddIngredient(ingredient);
			
			if (RecipesManager.Instance.GetCompleteRecipe(_ingredientsInside, out RecipeData recipe)){
				_recipeData = recipe;
				_bowlCanvas.UpdateRecipe(_recipeData.recipeSprite);
			}
		}
	}	
}
