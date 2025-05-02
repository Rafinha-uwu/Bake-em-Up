using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FryerBasket : ToolContainer
{
	[SerializeField]
	private ShapedDoughsSocketsManager _shapedDoughsSocketsManager;

	private MixerCanvas _fryerBasketCanvas;

	[HideInInspector]
	public bool HasCompletedBread = false;
	[HideInInspector]
	public bool HasBurnedBread = false;
	private Resettable _resettable;
	private bool _skipNextContainerEmptyMessage = false;

	protected override void Awake()
	{
		base.Awake();
		_fryerBasketCanvas = _toolCanvas as MixerCanvas;
		_resettable = GetComponent<Resettable>();
		_resettable.OnObjectReset += ClearBasket;
		_fryerBasketCanvas.DisableCanvas();
	}
	private void OnDestroy()
	{
		_resettable.OnObjectReset -= ClearBasket;
	}

	public bool GetRecipe(out RecipeData recipe)
	{
		recipe = _recipeData;

		return recipe != null;
	}

	public void MakeBread()
	{
		RecipeData auxRecipe = _recipeData;
		ClearBasket();

		HasCompletedBread = true;

		for (int i = 0; i < auxRecipe.shapedDoughCount; i++)
		{
			Instantiate(auxRecipe.breadPrefab, transform.position, Quaternion.identity);
		}
		_recipeData = auxRecipe;
	}

	public void BurnBread()
	{
		_skipNextContainerEmptyMessage = true;

		ClearBasket();

		HasBurnedBread = true;
		_recipeData = RecipesManager.Instance.GetBadBread();

		for (int i = 0; i < _recipeData.shapedDoughCount; i++)
		{
			Instantiate(_recipeData.breadPrefab, transform.position, Quaternion.identity);
		}
	}

	public void ClearBasket()
	{
		HasCompletedBread = false;
		HasBurnedBread = false;

		_shapedDoughsSocketsManager.DestroyAllDough();

		_fryerBasketCanvas.ClearCanvas();
	}

	public void SetCanvasRecipe(Sprite recipeSprite)
	{
		_fryerBasketCanvas.SetRecipe(recipeSprite);
	}

	public void UpdateCanvasTimer(float currentTimer, float maxTimer, float badMaxTimer)
	{
		_fryerBasketCanvas.UpdateTimer(currentTimer, maxTimer, badMaxTimer);
	}

	public void ClearCanvas()
	{
		_fryerBasketCanvas.ClearCanvas();
	}

	public override void ContainerIsEmpty()
	{
		if (_skipNextContainerEmptyMessage)
		{
			_skipNextContainerEmptyMessage = false;
			return;
		}

		_recipeData = null;
		_collider.enabled = true;

		ClearBasket();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_collider.enabled)
			return;

		RecipeData recipe = null;

		if (other.gameObject.CompareTag("Shaped Dough"))
		{
			recipe = other.gameObject.GetComponentInParent<ShapedDough>().GetRecipe();
			if (recipe.FryingTime == 0f)
				return;

			XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
			if (interactable.isSelected)
			{
				XRBaseInteractor interactor = interactable.firstInteractorSelecting as XRBaseInteractor;
				WoodenBoard board = interactor.GetComponentInParent<WoodenBoard>();
				if (!board)
					return;

				board.ReleaseAllDough();
			}
		}
		else if (other.gameObject.CompareTag("Bread"))
		{
			recipe = other.gameObject.GetComponentInParent<Bread>().GetRecipe();
			if (recipe.FryingTime == 0f)
				return;

			XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
			if (interactable.isSelected)
				return;
		}

		if (recipe != null)
		{
			_recipeData = recipe;
			_shapedDoughsSocketsManager.ReceivedItem();
			_collider.enabled = false;
		}
	}
}
