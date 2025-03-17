using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSimpleInteractable))]
public class IngredientContainerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _ingredient;

	private void Awake()
	{
		var interactable = GetComponent<XRSimpleInteractable>();
		interactable.selectEntered.AddListener(ContainerSelected);
	}

	private void OnDisable()
	{
		var interactable = GetComponent<XRSimpleInteractable>();
		interactable.selectEntered.RemoveListener(ContainerSelected);
	}

	private void ContainerSelected(SelectEnterEventArgs args)
    {
		IXRSelectInteractor interactor = args.interactorObject;
		XRInteractionManager interactionManager = args.manager;

		if (interactor != null)
		{
			GrabIngredient(interactor, interactionManager);
		}
	}

	private void GrabIngredient(IXRSelectInteractor interactor, XRInteractionManager interactionManager)
	{
		GameObject ingredient = Instantiate(_ingredient, transform.position, transform.rotation);
		
		if (ingredient.TryGetComponent<XRGrabInteractable>(out var newInteractable))
		{
			interactionManager.SelectExit(interactor, interactor.firstInteractableSelected);

			interactionManager.SelectEnter(interactor, newInteractable);
		}
	}
}
