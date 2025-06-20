using UnityEngine;

public class ScientistPhone : PhoneController
{
	protected override void Start()
	{
		base.Start();
		LevelManager.Instance.ScientistPhone = this;
	}
}
