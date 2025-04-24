using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientCanvasElement : MonoBehaviour
{
	[SerializeField]
	private Image _image;
	[SerializeField]
	private TMP_Text _textMeshPro;

	private void Start()
	{
		_textMeshPro.text = "1";
	}

	public void UpdateImage(Sprite sprite)
	{
		_image.sprite = sprite;
	}

	public void UpdateCount(int value)
	{
		_textMeshPro.text = value.ToString();
	}
}
