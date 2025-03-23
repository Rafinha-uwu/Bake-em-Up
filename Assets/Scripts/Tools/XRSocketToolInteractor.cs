using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRSocketToolInteractor : XRSocketInteractor
{
    [SerializeField]
    private ToolName _toolName;
	private IXRSelectInteractable _interactable;
	public IXRSelectInteractable Interactable => _interactable;

	public bool IsToolOn = false;

	protected override void OnEnable()
	{
		base.OnEnable();
		selectEntered.AddListener(SocketSelectEnter);
		selectExited.AddListener(SocketSelectExit);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		selectEntered.RemoveListener(SocketSelectEnter);
		selectExited.RemoveListener(SocketSelectExit);
	}

	public override bool CanHover(IXRHoverInteractable interactable)
	{
		bool canHover = base.CanHover(interactable);

		bool correctTool = false;
		if (interactable.transform.gameObject.TryGetComponent<ToolContainer>(out var tool))
		{
			if(tool.ToolName == _toolName)
				correctTool = true;
		}

		return canHover && correctTool && !IsToolOn;
	}

	public override bool CanSelect(IXRSelectInteractable interactable)
	{
		bool canSelect = base.CanSelect(interactable);

		bool correctTool = false;
		if (interactable.transform.gameObject.TryGetComponent<ToolContainer>(out var tool))
		{
			if (tool.ToolName == _toolName)
				correctTool = true;
		}

		return canSelect && correctTool && !IsToolOn;
	}

	private void SocketSelectEnter(SelectEnterEventArgs args)
	{
		if (_interactable != null)
			return;

		_interactable = args.interactableObject;
		if (_interactable.transform.gameObject.TryGetComponent<ToolContainer>(out var container))
			container.DisableCanvas();
	}

	private void SocketSelectExit(SelectExitEventArgs args)
	{
		if (_interactable == null || IsToolOn)
			return;

		if (_interactable.transform.gameObject.TryGetComponent<ToolContainer>(out var container))
			container.EnableCanvas();

		_interactable = null;
	}
}


