using UnityEngine;
using UnityEngine.UI;

public class LimitUIMaxSize : MonoBehaviour
{
	[SerializeField] private HorizontalOrVerticalLayoutGroup _layoutGroup;
	[SerializeField] private LayoutElement _presenterLE;
	[SerializeField] private RectTransform _icon;
	[SerializeField] private RectTransform _text;
	
	[SerializeField] private float _maxTotalWidth = 500f;

	private void Start()
	{
		_presenterLE.minWidth = _layoutGroup.padding.left + _layoutGroup.padding.right + _layoutGroup.spacing + (2 *_icon.rect.width);
	}

	private void Update()
	{
		float totalWidth = _layoutGroup.padding.left + _layoutGroup.padding.right + _layoutGroup.spacing + _icon.rect.width + _text.rect.width;
		if (totalWidth >= _maxTotalWidth)
		{
			_presenterLE.preferredWidth = _maxTotalWidth;
		}
		else
		{
			_presenterLE.preferredWidth = -1;
		}
	}
}
