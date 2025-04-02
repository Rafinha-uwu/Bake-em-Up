using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MixerCanvas : ToolCanvas
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
		_timerTMP.text = "0:00";
		_percentTMP.text = "0%";
	}
}
