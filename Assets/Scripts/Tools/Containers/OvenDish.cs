using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Resettable))]
public class OvenDish : ToolContainer
{
    [SerializeField]
    private ShapedDoughsSocketsManager _shapedDoughsSocketsManager;

	private bool _hasCompletedBread = false;
	private bool _hasBurnedBread = false;
	private bool _hasDough = false;
    public bool HasCompletedBread => _hasCompletedBread;
    public bool HasBurnedBread => _hasBurnedBread;
	public bool HasDough => _hasDough;
	
	private Resettable _resettable;

	public delegate void OvenDishHandler();
	public event OvenDishHandler OnOvenDishHasDough;
	public event OvenDishHandler OnOvenDishEmpty;

	public delegate void DishHelperHandler(OvenDish dish);
	public event DishHelperHandler OnShowHelper;

	protected void Awake()
    {
        _resettable = GetComponent<Resettable>();
        _resettable.OnObjectReset += ClearDish;
    }
    private void OnDestroy()
    {
        _resettable.OnObjectReset -= ClearDish;
		OnOvenDishHasDough = null;
		OnOvenDishEmpty = null;
		OnShowHelper = null;
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

		ClearDish();

		_hasCompletedBread = true;

		GameObject bread = burned ? auxRecipe.burnedBreadPrefab : auxRecipe.breadPrefab;

		for (int i = 0; i < breadCount; i++)
		{
			Instantiate(bread, transform.position, Quaternion.identity);
		}
	}

    public void ClearDish()
    {
        _shapedDoughsSocketsManager.DestroyAllDough();
    }

    public override void ContainerIsEmpty()
    {
		OnOvenDishEmpty?.Invoke();

		_recipeData = null;
		_hasCompletedBread = false;
		_hasBurnedBread = false;
		_hasDough = false;
	}

	private void OnTriggerEnter(Collider other)
    {
		GameObject item = other.gameObject;

		if (!_recipeData.IsUnityNull() && (item.CompareTag("Shaped Dough") || item.CompareTag("Bread")))
		{
			_shapedDoughsSocketsManager.OnContainerTriggerEnter(item);
			return;
		}

		RecipeData recipe = null;

		if (item.CompareTag("Shaped Dough"))
		{
			recipe = item.GetComponentInParent<ShapedDough>().GetRecipe();
			if (recipe.OvenTime == 0f)
				return;

			_hasDough = true;

			OnOvenDishHasDough?.Invoke();
			OnShowHelper?.Invoke(this);
		}
		else if (item.CompareTag("Bread"))
		{
			Bread bread = item.GetComponentInParent<Bread>();

			recipe = bread.GetRecipe();

			if (recipe.OvenTime == 0f)
				return;

			if (bread.IsBurned())
				_hasBurnedBread = true;
			else
				_hasCompletedBread = true;
		}

		if (recipe != null)
		{
			_recipeData = recipe;
			_shapedDoughsSocketsManager.ReceivedItem(recipe, item);
		}
	}
}
