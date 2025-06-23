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
    public bool activate = false;
    public bool single = false;

    public void ButtonPoke()
    {
        if (!_isPressed)
        {
            if (!single)
            {
                _isPressed = true;
            }
            else
            {
                activate = true;
                Invoke("D", .5f);
            }

            initialPosition = _onTransform.localPosition;
            _renderer.material = _onMaterial;
            OnTurnOn?.Invoke();
        }
        else
        {
            TurnOffButton();
            OnTurnOff?.Invoke();
        }
    }

    public void TurnOffButton()
    {
        _isPressed = false;
        initialPosition = _offTransform.localPosition;
        _renderer.material = _offMaterial;
    }

    private void D()
    {
        activate = false;
    }
}
