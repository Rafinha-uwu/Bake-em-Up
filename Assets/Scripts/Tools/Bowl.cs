using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable)), RequireComponent(typeof(Resettable))]
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
	private Resettable _resettable;

	protected override void Awake()
	{
		base.Awake();
		_bowlCanvas = _toolCanvas.gameObject.GetComponent<BowlCanvas>();
		_resettable = GetComponent<Resettable>();
		_resettable.OnObjectReset += ClearBowl;
	}

	private void OnDestroy()
	{
		_resettable.OnObjectReset -= ClearBowl;
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

		GameObject firstDough = Instantiate(_recipeData.doughPrefab, _container.transform.position, Quaternion.identity);
		InsertItem(firstDough);

		GameObject secondDough = Instantiate(_recipeData.doughPrefab, _container.transform.position, Quaternion.identity);
		InsertItem(secondDough);
	}

	public void MakeBadDough()
	{
		ClearBowl();

		HasBadDough = true;

		GameObject badDough = Instantiate(RecipesManager.Instance.GetBadDough(), _container.transform.position, Quaternion.identity);
		InsertItem(badDough);
	}

	public void ClearBowl()
	{
		HasCompletedDough = false;
		HasBadDough = false;

		_ingredientsInside.Clear();
		foreach (Transform child in _container.transform)
		{
			Destroy(child.gameObject);
		}
		_bowlCanvas.ClearIngredients();
	}

	private void OnTriggerEnter(Collider other)
	{
		var interactable = other.gameObject.GetComponentInParent<XRGrabInteractable>();

		if (interactable)
		{
			if (!interactable.isSelected)
				return;

			if (!interactable.firstInteractorSelecting.transform.CompareTag("Player"))
				return;

			ReleaseItem(interactable);
			InsertItem(interactable.gameObject);
		}
	}

	private void InsertItem(GameObject obj)
	{
		SetLayerAllChildren(obj.transform, "Inside Bowl");
		obj.transform.SetParent(_container.transform, true);
		obj.transform.localPosition = Vector3.zero;
		//obj.transform.SetPositionAndRotation(_container.transform.position, Quaternion.identity);

		if (obj.TryGetComponent<IngredientController>(out var ingredient))
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

	private void SetLayerAllChildren(Transform root, string layerName)
	{
		var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (var child in children)
		{
			child.gameObject.layer = LayerMask.NameToLayer(layerName);
		}
	}
}
