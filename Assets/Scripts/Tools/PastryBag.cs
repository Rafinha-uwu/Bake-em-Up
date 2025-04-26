using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable)), RequireComponent(typeof(Resettable))]
public class PastryBag : Tool
{
	private int _remainingCream = 0;
	private int _maxCream = 0;

	private RecipeData _recipeData;
	
	public void Shot()
	{
		Debug.Log($"Shot: {_remainingCream}");
		if (_remainingCream == 0)
			return;

		_remainingCream -= 1;
		if (_remainingCream == 0)
		{
			_recipeData = null;
			_maxCream = 0;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Cream"))
		{
			XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
			if (interactable == null || interactable.isSelected)
				return;

			PastryCream cream = other.gameObject.GetComponentInParent<PastryCream>();
			_recipeData = cream.GetRecipe();
			AddCream();

			Destroy(cream.gameObject);
			return;
		}
	}

	private void AddCream()
	{
		_remainingCream += _recipeData.shapedDoughCount;
		
		_maxCream = _recipeData.shapedDoughCount * 2;
		
		if(_remainingCream > _maxCream)
		{
			_remainingCream = _maxCream;
		}
	}
}
