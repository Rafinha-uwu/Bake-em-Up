using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MixerCanvas : ToolCanvas
{
	[SerializeField]
	private Image _recipeImage;
	[SerializeField]
	private Slider _clockslider;
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

	public void UpdateTimer(float currentTimer, float maxTimer)
	{
		int minutes = Mathf.FloorToInt(currentTimer / 60f);
		int seconds = Mathf.FloorToInt(currentTimer % 60f);

		_timerTMP.text = string.Format("{0:0}:{1:00}", minutes, seconds);

		float sliderValue = currentTimer / maxTimer;
		_clockslider.value = sliderValue;

		int percentage = Mathf.RoundToInt(sliderValue * 100f);
		_percentTMP.text = $"{percentage}%";
	}

	private void ClearRecipe()
	{
		_recipeImage.sprite = null;
		_clockslider.value = 0;
		_timerTMP.text = "0:00";
		_percentTMP.text = "0%";
	}
}
