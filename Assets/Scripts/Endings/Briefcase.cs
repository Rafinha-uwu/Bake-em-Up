using UnityEngine;

public class Briefcase : MonoBehaviour
{
    [SerializeField] private GameObject AirStrike;
    public void RainFire()
    {
        Instantiate(AirStrike, AirStrike.transform.position, AirStrike.transform.rotation);
    }
}
