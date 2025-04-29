using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Resettable))]
public class OvenDish : ToolContainer
{
    [SerializeField]
    private ShapedDoughsSocketsManager _shapedDoughsSocketsManager;

    private MixerCanvas _dishCanvas;

    [HideInInspector]
    public bool HasCompletedBread = false;
    [HideInInspector]
    public bool HasBurnedBread = false;
    private Resettable _resettable;
    private bool _skipNextContainerEmptyMessage = false;

    protected override void Awake()
    {
        base.Awake();
        _dishCanvas = _toolCanvas.gameObject.GetComponent<MixerCanvas>();
        _resettable = GetComponent<Resettable>();
        _resettable.OnObjectReset += ClearDish;
        _dishCanvas.DisableCanvas();
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

    public void MakeBread()
    {
        RecipeData auxRecipe = _recipeData;
		ClearDish();

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

		ClearDish();

        HasBurnedBread = true;
        _recipeData = RecipesManager.Instance.GetBadBread();

        for (int i = 0; i < _recipeData.shapedDoughCount; i++) 
        {
            Instantiate(_recipeData.breadPrefab, transform.position, Quaternion.identity);
        }
    }

    public void ClearDish()
    {
        HasCompletedBread = false;
        HasBurnedBread = false;

        _shapedDoughsSocketsManager.DestroyAllDough();

        _dishCanvas.ClearCanvas();
    }

    public void SetCanvasRecipe(Sprite recipeSprite)
    {
        _dishCanvas.SetRecipe(recipeSprite);
    }

    public void UpdateCanvasTimer(float currentTimer, float maxTimer, float badMaxTimer)
    {
        _dishCanvas.UpdateTimer(currentTimer, maxTimer, badMaxTimer);   
    }

    public void ClearCanvas()
    {
        _dishCanvas.ClearCanvas();
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

        ClearDish();
	}

    private void OnTriggerEnter(Collider other)
    {
		if (!_collider.enabled)
			return;

		RecipeData recipe = null;

        if (other.gameObject.CompareTag("Shaped Dough"))
        {
			recipe = other.gameObject.GetComponentInParent<ShapedDough>().GetRecipe();
            if (recipe.OvenTime == 0f)
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
			if (recipe.OvenTime == 0f)
				return;

			XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
            if (interactable.isSelected)
                return;
		}

        if(recipe != null)
        {
			_recipeData = recipe;
			_shapedDoughsSocketsManager.ReceivedItem();
			_collider.enabled = false;
		}
    }
}
