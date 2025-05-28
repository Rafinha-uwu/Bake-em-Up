using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable)), RequireComponent(typeof(Resettable))]
public class Bowl : ToolContainer
{
	[SerializeField]
	private GameObject _container;

	[SerializeField]
	private BowlCanvas _bowlCanvas;

	private Dictionary<IngredientName, int> _ingredientsInside = new();

	[HideInInspector]
	public bool HasCompletedDough = false;
	[HideInInspector]
	public bool HasBadDough = false;
	[HideInInspector]
	public bool HasRecipeReady = false;
	private Resettable _resettable;

	private int _doughCount = 0;
	private GameObject _dough = null;

	public delegate void BowlHandler();
	public event BowlHandler OnRecipeReady;
	public event BowlHandler OnRecipeNotReady;
	public event BowlHandler OnIngredientEntered;

	protected override void Awake()
	{
		base.Awake();
		_resettable = GetComponent<Resettable>();
		_resettable.OnObjectReset += ClearBowl;
	}

	private void OnDestroy()
	{
		_resettable.OnObjectReset -= ClearBowl;
		OnRecipeReady = null;
		OnRecipeNotReady = null;
		OnIngredientEntered = null;
	}

	public bool GetRecipe(out RecipeData recipe)
	{
		recipe = _recipeData;

		return recipe != null;
	}

	public void MakeDough()
	{
		RecipeData auxRecipe = _recipeData;
		ClearBowl();

		_recipeData = auxRecipe;

		HasCompletedDough = true;

		GameObject firstDough = Instantiate(_recipeData.doughPrefab, _container.transform.position, Quaternion.identity);
		InsertItem(firstDough);

		GameObject secondDough = Instantiate(_recipeData.doughPrefab, _container.transform.position, Quaternion.identity);
		InsertItem(secondDough);

		_dough = firstDough;

		_doughCount = 2;

		_bowlCanvas.UpdateRecipe(_recipeData.recipeSprite);
	}

	public void MakeBadDough()
	{
		ClearBowl();

		HasBadDough = true;

		_recipeData = RecipesManager.Instance.GetBadBread();
		GameObject badDough = Instantiate(_recipeData.doughPrefab, _container.transform.position, Quaternion.identity);
		InsertItem(badDough);

		_doughCount = 1;

		_bowlCanvas.UpdateRecipe(_recipeData.recipeSprite);
	}

	public void ClearBowl()
	{
		HasCompletedDough = false;
		HasBadDough = false;
		HasRecipeReady = false;

		_ingredientsInside.Clear();
		_recipeData = null;
		_dough = null;
		foreach (Transform child in _container.transform)
		{
			Destroy(child.gameObject);
		}
		_bowlCanvas.ClearCanvas();

		OnRecipeNotReady?.Invoke();
	}

	public void DoughRemoved()
	{
		_doughCount -= 1;

		if (_doughCount == 0)
			ClearBowl();
	}

	public GameObject GetDough()
	{
		return _dough;
	}

	public void EnableCanvas()
	{
		if (!_bowlCanvas.IsUnityNull())
			_bowlCanvas.EnableCanvas();
	}

	public void DisableCanvas()
	{
		if (!_bowlCanvas.IsUnityNull())
			_bowlCanvas.DisableCanvas();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (HasCompletedDough || HasBadDough)
			return;

		var interactable = other.gameObject.GetComponentInParent<XRGrabInteractable>();
		var ingredient = other.gameObject.GetComponentInParent<IngredientController>();
		var tool = other.gameObject.GetComponentInParent<Tool>();

		if (!interactable.IsUnityNull() && !ingredient.IsUnityNull() && tool.IsUnityNull())
		{
			if (interactable.isSelected)
			{
				if(interactable.firstInteractorSelecting.transform.CompareTag("Player"))
					ReleaseItem(interactable);

				return;
			}

			InsertItem(interactable.gameObject);
		}
	}

	private void InsertItem(GameObject obj)
	{
		SetLayerAllChildren(obj.transform, "Inside Bowl");
		obj.transform.SetParent(_container.transform, true);
		obj.transform.localPosition = Vector3.zero;

		if (obj.TryGetComponent<IngredientController>(out var ingredient))
		{
			obj.transform.localScale *= 0.5f;
			AddIngredient(ingredient);
		}
	}

	private void AddIngredient(IngredientController ingredient)
	{
		OnIngredientEntered?.Invoke();

		IngredientName name = ingredient.IngredientName;
		if (!_ingredientsInside.ContainsKey(name))
		{
			_ingredientsInside.Add(name, 1);
			_bowlCanvas.AddIngredient(ingredient);

			if (RecipesManager.Instance.GetCompleteRecipe(_ingredientsInside, out RecipeData recipe))
			{
				_recipeData = recipe;
				_bowlCanvas.UpdateRecipe(_recipeData.recipeSprite);
				OnRecipeReady?.Invoke();
				HasRecipeReady = true;
			}
		}
	}

	private void SetLayerAllChildren(Transform root, string layerName)
	{
		var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (var child in children)
		{
			child.gameObject.layer = LayerMask.NameToLayer(layerName);
		}
	}
}
