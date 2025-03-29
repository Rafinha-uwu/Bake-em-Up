using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRGrabWithSocketInteractable : XRGrabInteractable
{
    [SerializeField]
    private Transform _socketTransform;
	
	private Transform _firstAttachTransform;

	private void Start()
	{
		_firstAttachTransform = attachTransform;
	}

	protected override void OnSelectEntering(SelectEnterEventArgs args)
	{
		base.OnSelectEntering(args);
		if (_socketTransform != null && args.interactorObject is XRSocketInteractor)
		{
			attachTransform = _socketTransform;
		}
	}

	protected override void OnSelectExiting(SelectExitEventArgs args)
	{
		base.OnSelectExiting(args);
		if (_socketTransform != null && args.interactorObject is XRSocketInteractor)
		{
			attachTransform = _firstAttachTransform;
		}
	}
}
