using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ToolButton : XRPokeFollowAffordance
{
	public delegate void PressHandler();
	public event PressHandler OnTurnOn;
	public event PressHandler OnTurnOff;

	[SerializeField]
	private Transform _onTransform;
	[SerializeField]
	private Transform _offTransform;

	[SerializeField]
	private Material _onMaterial;
	[SerializeField]
	private Material _offMaterial;

	[SerializeField]
	private MeshRenderer _renderer;

	private bool _isPressed = false;
	public void ButtonPoke()
	{
		if (!_isPressed)
		{
			OnTurnOn?.Invoke();
			_isPressed = true;
			initialPosition = _onTransform.localPosition;
			_renderer.material = _onMaterial;
		}
		else
		{
			OnTurnOff?.Invoke();
			_isPressed = false;
			initialPosition = _offTransform.localPosition;
			_renderer.material = _offMaterial;
		}
	}
}
