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

	private XRBaseInteractable _interactable;

	private TutorialGrabStep _tutorialStep;

	private void Awake()
	{
		_interactable = GetComponent<XRSimpleInteractable>();
		_interactable.selectEntered.AddListener(ContainerSelected);
	}

	private void OnDestroy()
	{
		_interactable.selectEntered.RemoveListener(ContainerSelected);
	}

	public void SetTutorialStep(TutorialGrabStep tutorialGrabStep)
	{
		_tutorialStep = tutorialGrabStep;
	}

	private void ContainerSelected(SelectEnterEventArgs args)
    {
		if (args.interactableObject.IsSelectedByLeft() || args.interactableObject.IsSelectedByRight())
		{
			GameObject ingredient = Instantiate(_ingredient, transform.position, transform.rotation);

			if (!_tutorialStep.IsUnityNull())
			{
				XRBaseInteractable ingredientInteractable = ingredient.GetComponent<XRBaseInteractable>();
				ingredientInteractable.selectEntered.AddListener(IngredientSelected);
				ingredientInteractable.selectExited.AddListener(IngredientReleased);
			}

			if (ingredient.TryGetComponent<XRBaseInteractable>(out var newInteractable))
			{
				args.manager.SelectExit(args.interactorObject, args.interactableObject);

				args.manager.SelectEnter(args.interactorObject, newInteractable);
			}
		}
	}

	private void IngredientSelected(SelectEnterEventArgs args)
	{
		_tutorialStep.ItemGrabed(args);
	}

	private void IngredientReleased(SelectExitEventArgs args)
	{
		_tutorialStep.ItemReleased(args);
	}
}
