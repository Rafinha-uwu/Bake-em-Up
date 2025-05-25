using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ToolCanvas : MonoBehaviour
{
	private Transform _lookAt;
    private Transform _transformToFollow;
	private Canvas _canvas;
	
	protected virtual void Awake()
	{
		_canvas = GetComponent<Canvas>();
	}

	private void Start()
	{
		_lookAt = Camera.main.transform;
	}

	protected virtual void LateUpdate()
	{
		Vector3 newPosition = _transformToFollow ? _transformToFollow.position : transform.position;
		//if(Vector3.Distance(_transformToFollow.position, _lookAt.position) < 0.75f)
		//{
		//	Vector3 direction = (_transformToFollow.position - _lookAt.position).normalized;
		//	newPosition = _lookAt.position + direction * 0.75f;
		//}
		transform.position = newPosition;

		//transform.LookAt(_lookAt, Vector3.up);
		//transform.Rotate(0f, 180f, 0f);
	}

	public void AddTransformToFollow(Transform canvasPoint)
	{
		_transformToFollow = canvasPoint;
		transform.rotation = canvasPoint.rotation;
	}

	public void EnableCanvas()
	{
		_canvas.enabled = true;
	}

	public void DisableCanvas()
	{
		_canvas.enabled = false;
	}

	public virtual void ClearCanvas()
	{
		throw new NotImplementedException($"Clear Canvas for {gameObject.name} not implemented.");
	}
}
