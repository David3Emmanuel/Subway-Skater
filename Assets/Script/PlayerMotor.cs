using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    // Movement
    private CharacterController controller;
    private float jumpForce = 4.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private float speed = 7.0f;
    private const float TURN_SPEED = 0.05f;

    // Lane
    private int desiredLane = 1; // 0 = Left, 1 = Middle, 2 = Right
    private const float LANE_DISTANCE = 3.0f;

    // Animator
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (MobileInput.Instance.SwipeLeft) MoveLane(false);
        if (MobileInput.Instance.SwipeRight) MoveLane(true);

        Vector3 targetPosition = new(0, 0, transform.position.z);
        if (desiredLane == 0) targetPosition += Vector3.left * LANE_DISTANCE;
        else if (desiredLane == 2) targetPosition += Vector3.right * LANE_DISTANCE;

        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        animator.SetBool("Grounded", isGrounded);

        if (isGrounded)
        {
            verticalVelocity = -0.1f;

            if (MobileInput.Instance.SwipeUp)
            {
                animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity -= jumpForce;
            }
        }
        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        controller.Move(moveVector * Time.deltaTime);
        Vector3 direction = controller.velocity;
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, direction.normalized, TURN_SPEED);
        }
    }

    void MoveLane(bool isGoingRight)
    {
        desiredLane += isGoingRight ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    bool IsGrounded()
    {
        Ray groundRay = new(
            new Vector3(
                controller.bounds.center.x,
                controller.bounds.center.y - controller.bounds.extents.y + 0.2f,
                controller.bounds.center.z
            ),
            Vector3.down
        );

        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);
        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }
}
