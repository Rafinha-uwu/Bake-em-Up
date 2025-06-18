using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TutorialSocketStep : TutorialStep
{
	[SerializeField]
	private ContainerTool _containerTool;

	private enum ContainerTool { Bowl, Mixer, WoddenBoard, Oven };

	private bool _socketSelected = false;

	protected override void Start()
	{
		base.Start();

		switch (_containerTool)
		{
			case ContainerTool.Bowl:
				Bowl bowl = TutorialManager.Instance.Bowl;
				bowl.OnIngredientEntered += SocketSelected;
				return;

			case ContainerTool.Mixer:
				Mixer mixer = TutorialManager.Instance.Mixer;
				mixer.OnSocketSelected += SocketSelected;
				mixer.OnSocketExited += SocketReleased;
				break;

			case ContainerTool.WoddenBoard:
				WoodenBoard woodenBoard = TutorialManager.Instance.WoodBoard;
				woodenBoard.OnDoughOnBoard += SocketSelected;
				woodenBoard.OnDoughRemovedFromBoard += SocketReleased;
				break;

			case ContainerTool.Oven:
				Oven oven = TutorialManager.Instance.Oven;
				oven.OnDishInOven += SocketSelected;
				oven.OnDishExitedOven += SocketReleased;
				break;

			default:
				break;
		}
	}

	protected void OnDestroy()
	{
		switch (_containerTool)
		{
			case ContainerTool.Bowl:
				Bowl bowl = TutorialManager.Instance.Bowl;
				if(!bowl.IsUnityNull())
					bowl.OnIngredientEntered -= SocketSelected;
				return;

			case ContainerTool.Mixer:
				Mixer mixer = TutorialManager.Instance.Mixer;
				if (!mixer.IsUnityNull())
				{
					mixer.OnSocketSelected -= SocketSelected;
					mixer.OnSocketExited -= SocketReleased;

				}
				break;

			case ContainerTool.WoddenBoard:
				WoodenBoard woodenBoard = TutorialManager.Instance.WoodBoard;
				if (!woodenBoard.IsUnityNull())
				{
					woodenBoard.OnDoughOnBoard -= SocketSelected;
					woodenBoard.OnDoughRemovedFromBoard -= SocketReleased;
				}
				break;

			case ContainerTool.Oven:
				Oven oven = TutorialManager.Instance.Oven;
				if (!oven.IsUnityNull())
				{
					oven.OnDishInOven -= SocketSelected;
					oven.OnDishExitedOven -= SocketReleased;
				}
				break;

			default:
				break;
		}
	}

	public override void ShowStep(WorldIndicatorHelper indicatorHelper)
	{
		base.ShowStep(indicatorHelper);

		if (_socketSelected)
			StepCompleted();
	}

	private void SocketSelected()
	{
		if (_containerTool != ContainerTool.Bowl)
		{
			_socketSelected = true;
		}

		if (_isCurrentStep)
			StepCompleted();
	}

	private void SocketReleased()
	{
		_socketSelected = false;
		StepFailed();
	}
}
