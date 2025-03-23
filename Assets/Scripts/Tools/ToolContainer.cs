using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolContainer : MonoBehaviour
{
    [SerializeField]
    private ToolName _toolName;
	public ToolName ToolName => _toolName;

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

public enum ToolName { Bowl, OvenDish, FryingBasket }