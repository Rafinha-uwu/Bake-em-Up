using Unity.VisualScripting;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
	[SerializeField]
	private float _screenBorder = 1500f;

	[SerializeField]
	private GameObject _imageGroup;

	private RectTransform _canvas;
	private Transform _targetTransform;

	private void Awake()
	{
		_canvas = GetComponent<RectTransform>();
	}

	void LateUpdate()
	{
		if (_targetTransform.IsUnityNull())
			return;

		Vector3 targetViewportPoint = Camera.main.WorldToViewportPoint(_targetTransform.position);

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
		capped.y = Mathf.Clamp(capped.y, 0f + (_screenBorder / 2) / Screen.height, 1f - (_screenBorder / 2) / Screen.height);

		Vector3 clampedScreenPos = Camera.main.ViewportToScreenPoint(capped);

		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			_canvas, clampedScreenPos, null, out var localPoint);

		_imageGroup.transform.localPosition = localPoint;

		//Turn the canvas to the player
		_canvas.transform.LookAt(Camera.main.transform, Vector3.up);
		_canvas.transform.Rotate(0f, 180f, 0f); // Inverter se necessário
	}

	public void SetTargetPosition(Transform target)
	{
		_targetTransform = target;
	}

	public void RemoveTarget()
	{
		_targetTransform = null;
	}
}
