using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public abstract class ToolCooker : Tool
{
	[SerializeField]
	private Transform _transformForIndicatorHelper;
	protected WorldIndicatorHelper _warningHelper;
	[SerializeField]
	[Range(150, 200)]
	private int _badTimerPercent;
	public float BadTimerMultiplier 
	{ 
		get { return _badTimerPercent / 100f; }
	}

	protected ToolButton _toolButton;
	[SerializeField]
	protected XRSocketToolInteractor _socket;
	protected ToolCanvas _toolCanvas;

	protected virtual void Awake()
	{
		if (_badTimerPercent <= 100f)
		{
			throw new System.NotSupportedException($"Bad Timer Percent is {_badTimerPercent}%, needs to be more than 100%");
		}
	}

	protected virtual void Start()
	{
		_toolButton = GetComponentInChildren<ToolButton>();

		_warningHelper = GetComponentInChildren<WorldIndicatorHelper>();
		_warningHelper.SetTargetPosition(_transformForIndicatorHelper);
		_warningHelper.Hide();
	}

	public abstract void SocketSelectedEnter(XRSocketToolInteractor socket);
	public abstract void SocketSelectedExit(XRSocketToolInteractor socket);
	protected abstract void TurnOn();
	protected abstract void TurnOff();
}
