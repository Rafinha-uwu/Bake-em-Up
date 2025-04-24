using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolContainer : Tool
{
    [SerializeField]
    private ToolContainerName _toolContainerName;
	public ToolContainerName ToolContainerName => _toolContainerName;

	[SerializeField]
	protected GameObject _canvasObject;
	[SerializeField]
	private Transform _transformForCanvasToFollow;
	
	protected ToolCanvas _toolCanvas;

	protected virtual void Awake()
	{
		GameObject canvas = Instantiate(_canvasObject, transform.position, transform.rotation);
		_toolCanvas = canvas.GetComponent<ToolCanvas>();
		_toolCanvas.AddTransformToFollow(_transformForCanvasToFollow);
	}

	protected void ReleaseItem(XRGrabInteractable interactable)
	{
		XRInteractionManager interactionManager = interactable.interactionManager;

		interactionManager.SelectExit(interactable.firstInteractorSelecting, interactable);
	}

	public void EnableCanvas()
	{
		_toolCanvas.EnableCanvas();
	}

	public void DisableCanvas()
	{
		_toolCanvas.DisableCanvas();
	}
}

public enum ToolContainerName { Bowl, OvenDish, FryingBasket }