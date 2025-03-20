using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class ToolCanvas : MonoBehaviour
{
	private Transform _lookAt;
    private Transform _transformToFollow;

	private void Start()
	{
		_lookAt = Camera.main.transform;
	}

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
		transform.Rotate(0f, 180f, 0f);
	}

	public void AddTransformToFollow(Transform transform)
	{
		_transformToFollow = transform;
	}
}
