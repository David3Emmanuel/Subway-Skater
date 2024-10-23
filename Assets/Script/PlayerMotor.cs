using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private bool isRunning = false;

    // Movement
    private CharacterController controller;
    private float jumpForce = 6.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private const float TURN_SPEED = 0.05f;
    private float slideDuration = 1.0f;

    // Speed Modifier
    private float speed;
    private float initialSpeed = 7.0f;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    // Lane
    private int desiredLane = 1; // 0 = Left, 1 = Middle, 2 = Right
    private const float LANE_DISTANCE = 2.5f;

    // Animator
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        speed = initialSpeed;
    }

    public void StartRunning()
    {
        isRunning = true;
        animator.SetTrigger("Start Running");
    }

    void Update()
    {
        if (!isRunning) return;

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - initialSpeed);
        }

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
            verticalVelocity = -0.05f;

            if (MobileInput.Instance.SwipeUp)
            {
                animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                StartSliding();
                Invoke("StopSliding", slideDuration);
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
            // Snap to direction when close enough
            if (Vector3.Distance(transform.forward, direction.normalized) < 0.1f)
            {
                transform.forward = direction.normalized;
            }
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

    void StartSliding()
    {
        animator.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    void StopSliding()
    {
        animator.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Death();
                break;
            default:
                break;
        }
    }

    void Death()
    {
        isRunning = false;
        animator.SetTrigger("Death");
        GameManager.Instance.OnDeath();
    }
}
