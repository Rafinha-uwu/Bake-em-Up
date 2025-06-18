using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable)), RequireComponent(typeof(Resettable))]
public class Bowl : ToolContainer
{
	[SerializeField]
	private GameObject _container;

	[SerializeField]
	private BowlCanvas _bowlCanvas;

	[SerializeField]
	private Transform _canvasOffset;

	[SerializeField]
	private Mesh _defaultMesh;
	[SerializeField]
	private List<BowlDoughsMeshs> _bowlDoughMeshs = new();

	[Serializable]
	private class BowlDoughsMeshs
	{
		public RecipeData recipe;
		public Mesh mesh;
	}

	private Dictionary<IngredientName, int> _ingredientsInside = new();

	[HideInInspector]
	public bool HasCompletedDough = false;
	[HideInInspector]
	public bool HasCompletedCream = false;
	[HideInInspector]
	public bool HasBadDough = false;
	[HideInInspector]
	public bool HasRecipeReady = false;
	private Resettable _resettable;
	[SerializeField]
	private MeshFilter _filter;

	private int _doughCount = 0;
	private GameObject _dough = null;

	public delegate void BowlHandler();
	public event BowlHandler OnRecipeReady;
	public event BowlHandler OnRecipeNotReady;
	public event BowlHandler OnIngredientEntered;

	protected void Awake()
	{
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

		Mesh bowlMesh = _bowlDoughMeshs.FirstOrDefault(obj => obj.recipe == _recipeData).mesh;
		_filter.mesh = bowlMesh;
		
		_dough = _recipeData.doughPrefab;
			

		//GameObject firstDough = Instantiate(_recipeData.doughPrefab, _container.transform.position, Quaternion.identity);
		//InsertItem(firstDough);

		//GameObject secondDough = Instantiate(_recipeData.doughPrefab, _container.transform.position, Quaternion.identity);
		//InsertItem(secondDough);

		//_dough = firstDough;

		//_doughCount = 2;
		_doughCount = 1;

		_bowlCanvas.UpdateRecipe(_recipeData.recipeSprite);
	}

	public void MakeBadDough()
	{
		ClearBowl();

		HasBadDough = true;

		_recipeData = RecipesManager.Instance.GetBadBread();
		Mesh bowlMesh = _bowlDoughMeshs.FirstOrDefault(obj => obj.recipe == _recipeData).mesh;
		_filter.mesh = bowlMesh;
		//GameObject badDough = Instantiate(_recipeData.doughPrefab, _container.transform.position, Quaternion.identity);
		//InsertItem(badDough);

		_doughCount = 1;

		_bowlCanvas.UpdateRecipe(_recipeData.recipeSprite);
	}

	public void ClearBowl()
	{
		HasCompletedDough = false;
		HasCompletedCream = false;
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

		_filter.mesh = _defaultMesh;

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

	public void ResetCanvasPosition()
	{
		_bowlCanvas.AddTransformToFollow(_canvasOffset);
	}

	public void ChangeCanvasPosition(Transform canvasOffset)
	{
		_bowlCanvas.ChangeCanvasPosition(canvasOffset);
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

			InsertItem(ingredient.gameObject);
		}
	}

	private void InsertItem(GameObject obj)
	{
		GameObject auxObj = obj;

		if (obj.TryGetComponent<IngredientController>(out var ingredient))
		{
			auxObj = AddIngredient(ingredient);
			if (auxObj.IsUnityNull())
				return;

			auxObj.transform.localScale *= 0.5f;
		}

		SetLayerAllChildren(auxObj.transform, "Inside Bowl");
		auxObj.transform.SetParent(_container.transform, true);
		auxObj.transform.localPosition = Vector3.zero;
	}

	private GameObject AddIngredient(IngredientController ingredient)
	{
		GameObject ingredientVisual = null;
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

			ingredientVisual = Instantiate(ingredient.VisualPrefab);
		}

		Destroy(ingredient.gameObject);
		return ingredientVisual;
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