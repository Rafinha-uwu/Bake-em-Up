using UnityEngine;

public class MilitarPhone : PhoneController
{
    protected override void Start()
    {
		base.Start();
		LevelManager.Instance.MilitarPhone = this;
	}
}
