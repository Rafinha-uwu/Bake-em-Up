using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientCanvasElement : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	public void UpdateImage(Sprite sprite)
	{
		_image.sprite = sprite;
	}
}
