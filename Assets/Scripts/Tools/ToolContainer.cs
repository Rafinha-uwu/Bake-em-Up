using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;
using Unity.VisualScripting;

public class ToolContainer : Tool
{	
	protected RecipeData _recipeData;

	protected void ReleaseItem(XRGrabInteractable interactable)
	{
		if (!interactable.isSelected)
			return;

		interactable.interactionManager.SelectExit(interactable.firstInteractorSelecting, interactable);
	}

	public virtual void ContainerIsEmpty()
	{
		_recipeData = null;
	}

	public virtual bool HasPriorityOver(GameObject currentInteractor)
	{
		ToolContainer currentContainer = currentInteractor.GetComponentInParent<ToolContainer>();
		if (currentContainer == null)
			return true;
		
		if (_toolName == ToolName.Bowl 
			|| new[] { ToolName.PastryBag, ToolName.Balcony}.Contains(currentContainer.ToolName))
			return false;

		if (_toolName == ToolName.Balcony && currentContainer.ToolName == ToolName.Bowl)
			return false;

		if (_toolName == currentContainer.ToolName)
			return false;
		
		if (currentContainer.ToolName == ToolName.Bowl || _toolName == ToolName.Balcony)
			return true;

		if (currentContainer.ToolName == ToolName.WoodBoard && _toolName != ToolName.PastryBag)
			return true;

		if (_toolName == ToolName.OvenDish && _recipeData != null && _recipeData.OvenTime > 0f)
			return true;

		if (_toolName == ToolName.FryingBasket && _recipeData != null && _recipeData.FryingTime > 0f)
			return true;

		return false;
	}
}