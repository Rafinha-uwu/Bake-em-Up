using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    InputActionReference gripInput;
    InputActionReference triggerInput;

    private Animator animator;
    private void Awake()
    {
        // Initialize the animator component
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator) return
        float gripValue = gripInput.action.ReadValue<float>();
        float triggerValue = triggerInput.action.ReadValue<float>();

        animator.SetFloat("Grip", gripValue);
        animator.SetFloat("Trigger", triggerValue);
    }
}
