using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class Bowl : ToolContainer
{
	[SerializeField]
	private GameObject _container;

	private BowlCanvasManager _bowlCanvasManager;
	private RecipeData _recipeData;

	private Dictionary<IngredientName, int> _ingredientsInside = new();

	private bool _hasDough = false;

	protected override void Awake()
	{
		base.Awake();
		_bowlCanvasManager = _toolCanvas.gameObject.GetComponent<BowlCanvasManager>();
	}

	public bool GetRecipe(out RecipeData recipe)
	{
		recipe = _recipeData;

		return recipe != null;
	}

	public void MakeDough()
	{
		_hasDough = true;

		_ingredientsInside.Clear();
		foreach (Transform child in _container.transform)
		{
			Destroy(child.gameObject);
		}
		_bowlCanvasManager.ClearIngredients();

		GameObject dough = Instantiate(_recipeData.doughPrefab);
		InsertItem(dough);
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
			_bowlCanvasManager.UpdateIngredient(name, _ingredientsInside[name]);
		}
		else
		{
			_ingredientsInside.Add(name, 1);
			_bowlCanvasManager.AddIngredient(ingredient);
			
			if (RecipesManager.Instance.GetCompleteRecipe(_ingredientsInside, out RecipeData recipe)){
				_recipeData = recipe;
				_bowlCanvasManager.UpdateRecipe(_recipeData.recipeSprite);
			}
		}
	}	
}
