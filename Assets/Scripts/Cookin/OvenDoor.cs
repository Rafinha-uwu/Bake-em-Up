using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OvenDoor : MonoBehaviour
{

    public delegate void DoorHandler();
    public event DoorHandler OnClose;
    public event DoorHandler OnOpen;

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("OvenDoor"))
        {
            OnClose?.Invoke();
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("OvenDoor"))
        {
            OnOpen?.Invoke();
        }


    }
}
