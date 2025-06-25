using UnityEngine;

public class StartPhones : MonoBehaviour
{
	private bool hasExecuted = false;

	void LateUpdate()
	{
		if (!hasExecuted)
		{
			LevelEvents.PhonesStartRinging();
			hasExecuted = true;
		}
	}
}
