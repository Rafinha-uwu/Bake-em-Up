using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RecipeGrabbable : MonoBehaviour
{
    public bool IsGrabbed { get; private set; }

    private void Awake()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        IsGrabbed = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false; // Ensure movement while held
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        IsGrabbed = false;
    }
}
