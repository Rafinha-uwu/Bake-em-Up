using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WorldIndicatorHelper : MonoBehaviour
{
	[SerializeField]
	private Image _indicatorImage;
	[SerializeField]
    private RectTransform _pointRectTransform;
	[SerializeField]
	private float _screenBorder = 100f;
    
	private RectTransform _canvas;
    private Transform _targetPosition;

	private void Awake()
	{
		_canvas = GetComponent<RectTransform>();
	}

	void Update()
	{
		if (_targetPosition.IsUnityNull())
			return;

		Vector3 targetViewportPoint = Camera.main.WorldToViewportPoint(_targetPosition.position);

		bool isBehind = targetViewportPoint.z < 0;
		bool isOffScreen = targetViewportPoint.x < 0 || targetViewportPoint.x > 1f || targetViewportPoint.y < 0f || targetViewportPoint.y > 1f;

		Vector3 capped = targetViewportPoint;
		if (isBehind)
		{
			capped.x = 1f - targetViewportPoint.x;
			capped.y = 1f - targetViewportPoint.y;
			capped.z = Mathf.Abs(targetViewportPoint.z);

			//Need to change to the side, because when the player is turned back to the target the warning will appear in front of the player
			if (capped.x < 0.5f && capped.x >= 0f)
				capped.x = 0f;
			else if (capped.x > 0.5f && capped.x <= 1f)
				capped.x = 1f;
		}

		capped.x = Mathf.Clamp(capped.x, 0f + _screenBorder / Screen.width, 1f - _screenBorder / Screen.width);
		capped.y = Mathf.Clamp(capped.y, 0f + _screenBorder / Screen.height, 1f - _screenBorder / Screen.height);

		Vector3 clampedScreenPos = Camera.main.ViewportToScreenPoint(capped);

		//If is in front of player use the origin z distance to the camera
		if(isBehind || isOffScreen)
		{
			clampedScreenPos.z = 1f;
			RotatePointerTowardsTargetPosition(capped);
		}
		else
		{
			_pointRectTransform.localEulerAngles = Vector3.zero;
		}

		Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clampedScreenPos);			

		_canvas.position = worldPosition;

		//Turn the canvas to the player
		_canvas.transform.LookAt(Camera.main.transform, Vector3.up);
		_canvas.transform.Rotate(0f, 180f, 0f); // Inverter se necessário
	}

	private void RotatePointerTowardsTargetPosition(Vector3 viewPortPosition)
	{
		Vector3 toPosition = viewPortPosition;
		Vector3 fromPosition = new Vector3(0.5f, 0.5f, 0f);
		Vector3 dir = (toPosition - fromPosition).normalized;

		float angle = UtilsClass.GetAngleFromVectorFloat(dir);
		_pointRectTransform.localEulerAngles = new Vector3(0, 0, angle);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void SetIndicatorImage(Sprite sprite)
	{
		_indicatorImage.sprite = sprite;
	}

	public void SetTargetPosition(Transform target)
	{
		_targetPosition = target;
	}
}