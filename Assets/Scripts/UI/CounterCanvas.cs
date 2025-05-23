using TMPro;
using UnityEngine;

public class CounterCanvas : ToolCanvas
{
	[SerializeField]
	private TMP_Text _counterTMP;

	public override void ClearCanvas()
	{
		ClearRecipe();
	}

	public void UpdateCounter(int value)
	{
		_counterTMP.text = value.ToString();
	}

	private void ClearRecipe()
	{
		_counterTMP.text = "0";
	}
}
