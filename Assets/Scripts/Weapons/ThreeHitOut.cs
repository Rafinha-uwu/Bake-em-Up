using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;



public class ThreeHitOut : MonoBehaviour
{
    public int maxHits = 3;
    private int hitCount = 0;
    private XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;
    private bool isDestroyed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        Debug.Log("Baguette grabbed!");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie") && isGrabbed)
        {
            hitCount++;

            if (hitCount >= maxHits && !isDestroyed)
            {
                isDestroyed = true;
                StartCoroutine(DestroyAfterDelay());
            }
        }
        else if (other.CompareTag("Ground") && !isDestroyed)
        {
            isDestroyed = true;
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator DestroyAfterDelay()
    {

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}
