using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Read input axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0, vertical);
        move = Vector3.ClampMagnitude(move, 1f);

        // Face movement direction
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        // Set "setWalk" bool in Animator based on input magnitude
        if (animator != null)
        {
            bool isMoving = move.magnitude > 0.1f;
            animator.SetBool("Crawl", isMoving);
        }

        // Handle jump
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Final move
        Vector3 finalMove = (move * playerSpeed) + (Vector3.up * playerVelocity.y);
        controller.Move(finalMove * Time.deltaTime);
    }
}