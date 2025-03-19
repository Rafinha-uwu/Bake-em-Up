using Unity.VisualScripting;
using UnityEngine;

public class ToolCanvas : MonoBehaviour
{
	[SerializeField]
	private Transform _lookAt;
    [SerializeField]
    private Transform _transformToFollow;

	private void LateUpdate()
	{
		Vector3 newPosition = _transformToFollow.position;
		if(Vector3.Distance(_transformToFollow.position, _lookAt.position) < 1f)
		{
			Vector3 direction = (_transformToFollow.position - _lookAt.position).normalized;
			newPosition = _lookAt.position + direction * 1f;
		}
		transform.position = newPosition;

		transform.LookAt(_lookAt, Vector3.up);
	}
}
