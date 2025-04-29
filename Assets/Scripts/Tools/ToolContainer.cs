using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

public class ToolContainer : Tool
{
    [SerializeField]
    private ToolContainerName _toolContainerName;
	public ToolContainerName ToolContainerName => _toolContainerName;

	[SerializeField]
	protected GameObject _canvasObject;
	[SerializeField]
	protected Transform _transformForCanvasToFollow;
	
	protected ToolCanvas _toolCanvas;
	protected Collider _collider;
	protected RecipeData _recipeData;

	protected virtual void Awake()
	{
		_collider = GetComponent<Collider>();
		GameObject canvas = Instantiate(_canvasObject, transform.position, transform.rotation);
		_toolCanvas = canvas.GetComponent<ToolCanvas>();
		_toolCanvas.AddTransformToFollow(_transformForCanvasToFollow);
	}

	protected void ReleaseItem(XRGrabInteractable interactable)
	{
		if (!interactable.isSelected)
			return;

		interactable.interactionManager.SelectExit(interactable.firstInteractorSelecting, interactable);
	}

	public void EnableCanvas()
	{
		_toolCanvas.EnableCanvas();
	}

	public void DisableCanvas()
	{
		_toolCanvas.DisableCanvas();
	}

	public virtual void ContainerIsEmpty()
	{
		_collider.enabled = true;
		_recipeData = null;
	}

	public virtual bool HasPriorityOver(GameObject currentInteractor)
	{
		ToolContainer currentContainer = currentInteractor.GetComponent<ToolContainer>();
		if (currentContainer == null)
			return true;
		
		if (_toolContainerName == ToolContainerName.Bowl 
			|| new[] { ToolContainerName.PastryBag, ToolContainerName.Balcony}.Contains(currentContainer.ToolContainerName))
			return false;
		
		if (currentContainer.ToolContainerName == ToolContainerName.Bowl || _toolContainerName == ToolContainerName.Balcony)
			return true;

		if (currentContainer.ToolContainerName == ToolContainerName.WoodBoard && _toolContainerName != ToolContainerName.PastryBag)
			return true;

		if (_toolContainerName == ToolContainerName.OvenDish && _recipeData != null && _recipeData.OvenTime > 0f)
			return true;

		if (_toolContainerName == ToolContainerName.FryingBasket && _recipeData != null && _recipeData.FryingTime > 0f)
			return true;

		return false;
	}
}

public enum ToolContainerName { Bowl, OvenDish, FryingBasket, PastryBag, WoodBoard, Balcony }