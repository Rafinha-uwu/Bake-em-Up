using Unity.VisualScripting;
using UnityEngine;

public class FryerBasket : ToolContainer
{
	[SerializeField]
	private ShapedDoughsSocketsManager _shapedDoughsSocketsManager;

	private bool _hasCompleteBread = false;
	private bool _hasBurnedBread = false;
	private bool _hasDough = false;
	public bool HasCompletedBread => _hasCompleteBread;
	public bool HasBurnedBread => _hasBurnedBread;
	public bool HasDough => _hasDough;
	
	private Resettable _resettable;

	public delegate void BasketHandler();
	public event BasketHandler OnBasketHasDough;
	public event BasketHandler OnBasketEmpty;

	protected void Awake()
	{
		_resettable = GetComponent<Resettable>();
		_resettable.OnObjectReset += ClearBasket;
	}
	private void OnDestroy()
	{
		_resettable.OnObjectReset -= ClearBasket;
		OnBasketHasDough = null;
		OnBasketEmpty = null;
	}

	public bool GetRecipe(out RecipeData recipe)
	{
		recipe = _recipeData;

		return recipe != null;
	}

	public void MakeBread(bool burned = false)
	{
		int breadCount = _shapedDoughsSocketsManager.GetSocketsInUse();

		RecipeData auxRecipe = _recipeData;

		ClearBasket();

		_hasCompleteBread = true;

		GameObject bread = burned ? auxRecipe.burnedBreadPrefab : auxRecipe.breadPrefab;

		for (int i = 0; i < breadCount; i++)
		{
			Instantiate(bread, transform.position, Quaternion.identity);
		}
	}

	public void ClearBasket()
	{
		_shapedDoughsSocketsManager.DestroyAllDough();
	}

	public override void ContainerIsEmpty()
	{
		OnBasketEmpty?.Invoke();

		_recipeData = null;

		_hasCompleteBread = false;
		_hasBurnedBread = false;
		_hasDough = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject item = other.gameObject;

		if (!_recipeData.IsUnityNull())
		{
			_shapedDoughsSocketsManager.OnContainerTriggerEnter(item);
			return;
		}

		RecipeData recipe = null;

		if (item.CompareTag("Shaped Dough"))
		{
			recipe = item.GetComponentInParent<ShapedDough>().GetRecipe();
			if (recipe.FryingTime == 0f)
				return;

			_hasDough = true;
			OnBasketHasDough?.Invoke();
		}
		else if (item.CompareTag("Bread"))
		{
			Bread bread = item.GetComponentInParent<Bread>();

			recipe = bread.GetRecipe();

			if (recipe.FryingTime == 0f)
				return;

			if (bread.IsBurned())
				_hasBurnedBread = true;
			else
				_hasCompleteBread = true;
		}

		if (recipe != null)
		{
			_recipeData = recipe;
			_shapedDoughsSocketsManager.ReceivedItem(recipe, item);
		}
	}
}
