using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class RecipeContainer : MonoBehaviour
{
	[SerializeField]
	private RecipeData _breadRecipe;
	[SerializeField]
	private CounterCanvas _containerCanvas;

	private XRBaseInteractable _interactable;

	[SerializeField]
    private int _recipeCount = 0;

    void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
		_interactable.selectEntered.AddListener(ContainerSelected);

		if (_recipeCount == 0)
            HideContainer();
    }

	private void OnDestroy()
	{
		_interactable.selectEntered.RemoveListener(ContainerSelected);
	}

	public RecipeData GetRecipe()
	{
		return _breadRecipe;
	}

	public void AddRecipe()
    {
        _recipeCount += 1;
        _containerCanvas.UpdateCounter(_recipeCount);
        
        ShowContainer();
	}

	private void ContainerSelected(SelectEnterEventArgs args)
	{
		if( _recipeCount == 0)
			throw new ArgumentException($"RecipeContainer ({transform.name}): has no bread left, but player is trying to interact");

		if (args.interactableObject.IsSelectedByLeft() || args.interactableObject.IsSelectedByRight())
		{
			Vector3 instantiatePosition = transform.position;
			instantiatePosition.y = transform.position.y + 0.1f;

			GameObject bread = Instantiate(_breadRecipe.breadPrefab, instantiatePosition, transform.rotation);

			if (bread.TryGetComponent<XRBaseInteractable>(out var breadInteractable))
			{
				args.manager.SelectExit(args.interactorObject, args.interactableObject);

				args.manager.SelectEnter(args.interactorObject, breadInteractable);
			}

			_recipeCount -= 1;
			_containerCanvas.UpdateCounter(_recipeCount);
			if (_recipeCount == 0)
				HideContainer();
		}
	}

	private void ShowContainer()
    {
        gameObject.SetActive(true);
    }

    private void HideContainer()
    {
        gameObject.SetActive(false);
    }
}
