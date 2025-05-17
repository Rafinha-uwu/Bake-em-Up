using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TutorialGrabStep : TutorialStep
{
	[SerializeField]
	private GrabbableTool _grabbableTool;

	private enum GrabbableTool { Bowl, Ingredients, RollingPin, OvenDish, OvenDoor };

    private bool _isGrabed = false;
	private XRBaseInteractable _interactable;

	//Necessary for the OvenDishes
	private XRBaseInteractable _auxInteractable;

	//Necessary for the ingredients
	private bool _leftHandIngredient;
	private bool _righHandIngredient;

	protected override void Start()
	{
		base.Start();

		switch (_grabbableTool)
		{
			case GrabbableTool.Bowl:
				_interactable = TutorialManager.Instance.Bowl.GetComponent<XRBaseInteractable>();
				break;

			case GrabbableTool.Ingredients:
				foreach(var ingredient in TutorialManager.Instance.Ingredients)
				{
					ingredient.SetTutorialStep(this);
				}
				return;

			case GrabbableTool.RollingPin:
				_interactable = TutorialManager.Instance.RollingPin.GetComponent<XRBaseInteractable>();
				break;

			case GrabbableTool.OvenDish:
				_interactable = TutorialManager.Instance.OvenDish1.GetComponent<XRBaseInteractable>();
				_auxInteractable = TutorialManager.Instance.OvenDish2.GetComponent<XRBaseInteractable>();
				break;

			case GrabbableTool.OvenDoor:
				_interactable = TutorialManager.Instance.OverDoor.GetComponent<XRBaseInteractable>();
				break;

			default:
				break;
		}
		_interactable.selectEntered.AddListener(ItemGrabed);
		_interactable.selectExited.AddListener(ItemReleased);

		if (!_auxInteractable.IsUnityNull())
		{
			_auxInteractable.selectEntered.AddListener(ItemGrabed);
			_auxInteractable.selectExited.AddListener(ItemReleased);
		}
	}

	private void OnDestroy()
	{
		if (_grabbableTool == GrabbableTool.Ingredients) return;

		_interactable.selectEntered.RemoveListener(ItemGrabed);
		_interactable.selectExited.RemoveListener(ItemReleased);
		
		if (!_auxInteractable.IsUnityNull())
		{
			_auxInteractable.selectEntered.RemoveListener(ItemGrabed);
			_auxInteractable.selectExited.RemoveListener(ItemReleased);
		}
	}

	public override void ShowStep(WorldIndicatorHelper indicatorHelper)
	{
		base.ShowStep(indicatorHelper);

		if (_isGrabed)
			StepCompleted();
	}

	public void ItemGrabed(SelectEnterEventArgs args)
	{
		bool left = args.interactableObject.IsSelectedByLeft();
		bool right = args.interactableObject.IsSelectedByRight();

		if (_grabbableTool == GrabbableTool.Ingredients)
		{
			if (left)
				_leftHandIngredient = true;
			else if(right)
				_righHandIngredient= true;
		}

		if (left || right)
		{
			_isGrabed = true;

			if (_isCurrentStep)
				StepCompleted();
		}
	}

	public void ItemReleased(SelectExitEventArgs args)
	{
		bool left = args.interactorObject.handedness == InteractorHandedness.Left;
		bool right = args.interactorObject.handedness == InteractorHandedness.Right;

		if (_grabbableTool == GrabbableTool.Ingredients)
		{
			if (left)
			{
				_leftHandIngredient = false;
				if (_righHandIngredient)
					return;
			}
			else if (right)
			{
				_righHandIngredient = false;
				if (_leftHandIngredient)
					return;
			}
		}

		if (left || right)
		{
			_isGrabed = false;
			StartCoroutine(DetectIfChangedInteractor(args.interactableObject));
		}
	}

	private IEnumerator DetectIfChangedInteractor(IXRSelectInteractable interactable)
	{
		yield return new WaitForEndOfFrame();

		if (!interactable.isSelected)
		{
			StepFailed();
		}
	}
}
