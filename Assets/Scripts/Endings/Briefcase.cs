using Unity.VisualScripting;
using UnityEngine;

public class Briefcase : MonoBehaviour
{
    [SerializeField] private GameObject AirStrike;

    private bool cooldown;
    [SerializeField] private float CooldownTime = 0.5f;
    private float CoolTime = 60f;

    public void Update()
    {
        if (CoolTime > 0)
        {
            CoolTime -= Time.deltaTime;
            cooldown = true;
        }
        else if (CoolTime <= 0f)
        {
            cooldown = false;
        }
    }
    public void RainFire()
    {
        if (!cooldown)
        {
            Instantiate(AirStrike, AirStrike.transform.position, AirStrike.transform.rotation);
        }
    }
}
