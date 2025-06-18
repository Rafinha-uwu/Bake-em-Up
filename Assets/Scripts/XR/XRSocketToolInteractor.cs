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

	private ToolCooker _cooker;

	protected override void Start()
	{
		base.Start();
		_cooker = GetComponentInParent<ToolCooker>();
	}

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
		if (interactable.transform.gameObject.TryGetComponent<Tool>(out var tool))
		{
			if(tool.ToolName == _toolName)
				correctTool = true;
		}

		return canHover && correctTool;
	}

	public override bool CanSelect(IXRSelectInteractable interactable)
	{
		bool canSelect = base.CanSelect(interactable);

		bool correctTool = false;
		if (interactable.transform.gameObject.TryGetComponent<Tool>(out var tool))
		{
			if (tool.ToolName == _toolName)
				correctTool = true;
		}

		return canSelect && correctTool;
	}

	private void SocketSelectEnter(SelectEnterEventArgs args)
	{
		if (_cooker == null || _interactable != null)
			return;

		_interactable = args.interactableObject;
		
		_cooker.SocketSelectedEnter(this);
	}

	private void SocketSelectExit(SelectExitEventArgs args)
	{
		if (_cooker == null || _interactable == null)
			return;

		_cooker.SocketSelectedExit(this);

		_interactable = null;
	}
}


