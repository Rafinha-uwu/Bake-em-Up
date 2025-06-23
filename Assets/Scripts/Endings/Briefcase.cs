using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Briefcase : MonoBehaviour
{
    [SerializeField] private GameObject AirStrike;

    private bool cooldown;
    [SerializeField] private float CooldownTime = 60f;
    private float CoolTime = 60f;

    [SerializeField] private GameObject Button;
    private ToolButton toolButton;

    public void Start()
    {
        toolButton = Button.GetComponent<ToolButton>();
        CoolTime = CooldownTime;
    }
    public void Update()
    {
        if (CoolTime > 0)
        {
            CoolTime -= Time.deltaTime;
            cooldown = true;
        }
        else if (CoolTime <= 0f)
        {
            if (toolButton.activate)
            {
                cooldown = false;
                RainFire();
            }
        }
    }
    public void RainFire()
    {
        if (!cooldown)
        {
            Instantiate(AirStrike, AirStrike.transform.position, AirStrike.transform.rotation);
            toolButton.activate = false;
            CoolTime = CooldownTime;
        }
    }
}
