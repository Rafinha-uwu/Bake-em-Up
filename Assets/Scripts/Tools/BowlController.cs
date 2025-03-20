using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class BowlController : MonoBehaviour
{
	[SerializeField]
	private GameObject _bowlCanvas;
	[SerializeField]
	private GameObject _container;
	[SerializeField]
	private Transform _transformForCanvasToFollow;

	private BowlCanvasManager _bowlCanvasManager;
	private RecipeData _recipeData;

	private Dictionary<IngredientName, int> _ingredientsInside = new();

	private void Awake()
	{
		GameObject bowlCanvas = Instantiate(_bowlCanvas, transform.position, transform.rotation);

		bowlCanvas.GetComponent<ToolCanvas>().AddTransformToFollow(_transformForCanvasToFollow);

		_bowlCanvasManager = bowlCanvas.GetComponent<BowlCanvasManager>();
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
		obj.transform.SetParent(_container.transform, false);
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

	private void ReleaseItem(XRGrabInteractable interactable)
	{
		XRInteractionManager interactionManager = interactable.interactionManager;

		interactionManager.SelectExit(interactable.firstInteractorSelecting, interactable);
	}
}
