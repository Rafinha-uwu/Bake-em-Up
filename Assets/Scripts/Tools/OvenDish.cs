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
    private RecipeData _recipeData;

    [HideInInspector]
    public bool HasCompletedBread = false;
    [HideInInspector]
    public bool HasBurnedBread = false;
    private Resettable _resettable;

    private Collider _collider;

    protected override void Awake()
    {
        base.Awake();
        _collider = GetComponent<Collider>();
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
        ClearDish();

        HasCompletedBread = true;

        for (int i = 0; i < _recipeData.shapedDoughCount; i++)
        {
            Instantiate(_recipeData.breadPrefab, transform.position, Quaternion.identity);
        }

    }

    public void BurnBread()
    {
        ClearDish();

        HasBurnedBread = true;

        for (int i = 0; i < _recipeData.shapedDoughCount; i++) 
        {
            Instantiate(RecipesManager.Instance.GetBurnedBread(), transform.position, Quaternion.identity);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shaped Dough"))
        {
            XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
            if (interactable.isSelected)
            {
                XRBaseInteractor interactor = interactable.firstInteractorSelecting as XRBaseInteractor;
                WoodenBoard board = interactor.GetComponentInParent<WoodenBoard>();
                if (!board)
                    return;

                board.ReleaseAllDough();

            }

            _recipeData = other.gameObject.GetComponentInParent<ShapedDough>().GetRecipe();
            _shapedDoughsSocketsManager.ReceivedItem();
            _collider.enabled = false;
        }
        else if (other.gameObject.CompareTag("Bread"))
        {
            XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
            if (interactable.isSelected)
                return;

			_recipeData = other.gameObject.GetComponentInParent<Bread>().GetRecipe();
			_shapedDoughsSocketsManager.ReceivedItem();
			_collider.enabled = false;
		}
    }
}
