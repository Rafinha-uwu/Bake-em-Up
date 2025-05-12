using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Resettable))]
public class OvenDish : ToolContainer
{
    [SerializeField]
    private ShapedDoughsSocketsManager _shapedDoughsSocketsManager;

	[HideInInspector]
    public bool HasCompletedBread = false;
    [HideInInspector]
    public bool HasBurnedBread = false;
    private Resettable _resettable;

	protected override void Awake()
    {
        base.Awake();
        _resettable = GetComponent<Resettable>();
        _resettable.OnObjectReset += ClearDish;
    }
    private void OnDestroy()
    {
        _resettable.OnObjectReset -= ClearDish;
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

		HasCompletedBread = true;

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
		_recipeData = null;

		HasCompletedBread = false;
		HasBurnedBread = false;
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
			if (recipe.OvenTime == 0f)
				return;
		}
		else if (item.CompareTag("Bread"))
		{
			Bread bread = item.GetComponentInParent<Bread>();

			recipe = bread.GetRecipe();

			if (recipe.OvenTime == 0f)
				return;

			if (bread.IsBurned())
				HasBurnedBread = true;
			else
				HasCompletedBread = true;
		}

		if (recipe != null)
		{
			_recipeData = recipe;
			_shapedDoughsSocketsManager.ReceivedItem(recipe, item);
		}
	}
}
