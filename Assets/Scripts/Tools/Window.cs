using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Window : MonoBehaviour
{
	private void OnTriggerExit(Collider other)
	{
		if (other.transform.root.TryGetComponent<Resettable>(out var objToReset))
		{
			Vector3 worldDirection = other.ClosestPoint(transform.position) - transform.position;
			Vector3 localDirection = transform.InverseTransformDirection(worldDirection.normalized);

			if (LeftInterior(localDirection))
			{
				objToReset.ResetObject(true);
			}
			else
			{
				objToReset.CancelReset();
			}
		}
	}

	private bool LeftInterior(Vector3 localDirection)
	{
		bool isInFront = Vector3.Dot(Vector3.forward, localDirection) > 0.0f;
		return isInFront;
	}
}
