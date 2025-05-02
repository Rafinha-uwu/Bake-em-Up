using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRBreadInteractable : XRGrabInteractable
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
		if(args.interactorObject is XRSocketInteractor && _socketTransform != null)
		{
			attachTransform = _socketTransform;
		}

		base.OnSelectEntering(args);
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
