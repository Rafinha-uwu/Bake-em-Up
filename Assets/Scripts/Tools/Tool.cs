using UnityEngine;

public class Tool : MonoBehaviour
{
	[SerializeField]
	protected ToolName _toolName;
	public ToolName ToolName => _toolName;
}

public enum ToolName { Bowl, OvenDish, FryingBasket, PastryBag, WoodBoard, Balcony, Mixer, Oven, Fryer, RollingPin, Recipe }