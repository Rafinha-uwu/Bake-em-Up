using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRHandBasedInteractable : XRGrabInteractable
{
	[SerializeField]
	private Transform _rightHandAttachTransform;
	private Transform _leftHandAttachTransform;

	private void Start()
	{
		_leftHandAttachTransform = attachTransform;
	}

	protected override void OnSelectEntering(SelectEnterEventArgs args)
	{
		if (args.interactorObject.transform.CompareTag("Player"))
		{
			if (args.interactorObject.handedness == InteractorHandedness.Left)
				attachTransform = _leftHandAttachTransform;
			else if(args.interactorObject.handedness == InteractorHandedness.Right)
				attachTransform = _rightHandAttachTransform;
		}

		base.OnSelectEntering(args);
	}
}
