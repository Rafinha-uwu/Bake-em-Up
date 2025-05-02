using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class RagdollPart : MonoBehaviour
{
    public Rigidbody rb { get; private set; }
    public Collider col { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.isKinematic = true;
        col.enabled = false;
    }

    public void Activate()
    {
        rb.isKinematic = false;
        col.enabled = true;
    }

    public void AddHitForce(Vector3 force)
    {
        Activate();
        rb.AddForce(force, ForceMode.Impulse);
    }
}