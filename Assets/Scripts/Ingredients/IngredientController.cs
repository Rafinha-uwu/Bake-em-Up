using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class IngredientController : MonoBehaviour
{
    [SerializeField]
    private IngredientName _ingredientName;

	[SerializeField]
	private Sprite _ingredientIcon;

	public Sprite IngredientIcon => _ingredientIcon;
	public IngredientName IngredientName => _ingredientName;

	private void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
	}
}

public enum IngredientName { Flour, Salt, Sugar, Water, Garlic, Egg, Butter, Cheese, Milk, Vanilla, FoodColloring, DulceDeLeche }
