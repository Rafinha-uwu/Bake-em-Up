using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CookerCanvas : ToolCanvas
{
	[SerializeField]
	private Image _recipeImage;
	[SerializeField]
	private Slider _clockSlider;
	[SerializeField]
	private Slider _badClockSlider;
	[SerializeField]
	private TMP_Text _timerTMP;
	[SerializeField]
	private TMP_Text _percentTMP;

	[Header("Scale Settings")]
	[SerializeField]
	private AnimationCurve _curve;
	[SerializeField]
	[Range(1f, 2f)]
	private float _scaleMultiplier = 1.5f;
	[SerializeField]
	[Range(0f, 2f)]
	private float _durationScale = 0.15f;
	private RectTransform _rectTransform;
	private XRSimpleInteractable _interactable;
	private Vector3 _initialScale;
	private Vector3 _maxScale;
	private bool _increaseScale = false;
	private bool _finishedScaling = true;
	private float _elapsedScaleTime = 0f;

	protected override void Awake()
	{
		base.Awake();
		_rectTransform = GetComponent<RectTransform>();
		_interactable = GetComponent<XRSimpleInteractable>();
		if (!_interactable.IsUnityNull())
		{
			_interactable.hoverEntered.AddListener(IncreaseCanvas);
			_interactable.hoverExited.AddListener(DecreaseCanvas);

			_initialScale = _rectTransform.localScale * 1f;
			_maxScale = _initialScale * _scaleMultiplier;
		}
	}

	private void OnDestroy()
	{
		if (!_interactable.IsUnityNull())
		{
			_interactable.hoverEntered.RemoveListener(IncreaseCanvas);
			_interactable.hoverExited.RemoveListener(DecreaseCanvas);
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();

		if (_finishedScaling)
			return;

		if (_increaseScale)
			ScaleCanvas(_initialScale, _maxScale);
		else
			ScaleCanvas(_maxScale, _initialScale);
	}

	public override void ClearCanvas()
	{
		ClearRecipe();
	}

	public void SetRecipe(Sprite recipeSprite)
	{
		_recipeImage.sprite = recipeSprite;
	}

	public void UpdateTimer(float currentTimer, float maxTimer, float badMaxTimer)
	{
		if (maxTimer < 0)
			return;

		int minutes = Mathf.FloorToInt(currentTimer / 60f);
		int seconds = Mathf.FloorToInt(currentTimer % 60f);

		_timerTMP.text = string.Format("{0:0}:{1:00}", minutes, seconds);

		float sliderValue = currentTimer / maxTimer;
		_clockSlider.value = sliderValue;

		if(currentTimer >= maxTimer)
		{
			float badSliderValue = (currentTimer - maxTimer) / (badMaxTimer - maxTimer);
			_badClockSlider.value = badSliderValue;
		}

		int percentage = Mathf.RoundToInt(sliderValue * 100f);
		_percentTMP.text = $"{percentage}%";
	}

	private void ClearRecipe()
	{
		_recipeImage.sprite = null;
		_clockSlider.value = 0;
		_badClockSlider.value = 0;
		_timerTMP.text = "0:00";
		_percentTMP.text = "0%";
	}

	private void IncreaseCanvas(HoverEnterEventArgs args)
	{
		_finishedScaling = false;
		_increaseScale = true;
		_elapsedScaleTime = 0f;
	}

	private void DecreaseCanvas(HoverExitEventArgs args)
	{
		_finishedScaling = false;
		_increaseScale = false;
		_elapsedScaleTime = 0f;
	}

	private void ScaleCanvas(Vector3 fromScale, Vector3 targetScale)
	{
		if (_elapsedScaleTime == 0f)
		{
			float u = UtilsClass.InverseLerpFast(fromScale, targetScale, _rectTransform.localScale);
			_elapsedScaleTime = Mathf.Lerp(0f, _durationScale, u);
		}

		_elapsedScaleTime += Time.deltaTime;
		float t = Mathf.Clamp01(_elapsedScaleTime / _durationScale);

		float curvedT = _curve.Evaluate(t);

		transform.localScale = Vector3.Lerp(fromScale, targetScale, curvedT);

		if (t >= 1f)
			_finishedScaling = true;
	}
}
