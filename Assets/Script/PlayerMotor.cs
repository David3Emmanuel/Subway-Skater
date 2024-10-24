using System;
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
    private float slideDuration = 1.0f;

    // Speed Modifier
    private float speed;
    private float initialSpeed = 7.0f;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    // Lane
    private int desiredLane = 0; // -1 = Left, 0 = Middle, 1 = Right
    private const float LANE_DISTANCE = 2.5f;

    // Animator
    private Animator animator;

    // Audio
    public AudioClip switchLaneSFX, jumpSFX, slideSFX;
    private AudioSource audioSource;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        speed = initialSpeed;
    }

    public void StartRunning()
    {
        isRunning = true;
        animator.SetTrigger("Start Running");
    }

    void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
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

        bool isGrounded = IsGrounded();
        animator.SetBool("Grounded", isGrounded);

        if (isGrounded)
        {
            verticalVelocity = -0.1f;

            if (MobileInput.Instance.SwipeUp)
            {
                PlaySound(jumpSFX);
                animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                PlaySound(slideSFX);
                StartSliding();
                Invoke(nameof(StopSliding), slideDuration);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
            if (MobileInput.Instance.SwipeDown) verticalVelocity -= jumpForce;
        }

        Vector3 targetPosition = new(LANE_DISTANCE * desiredLane, 0f, transform.position.z);
        if (Math.Abs(transform.position.x - targetPosition.x) < 0.1f)
        {
            transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        }

        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;
        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        controller.Move(moveVector * Time.deltaTime);
        Vector3 direction = controller.velocity;
        if (direction != Vector3.zero)
        {
            direction.y = 0f;
            transform.forward = Vector3.Lerp(transform.forward, direction.normalized, Time.deltaTime);
        }
    }

    void MoveLane(bool isGoingRight)
    {
        desiredLane += isGoingRight ? 1 : -1;
        if (desiredLane < -1) desiredLane = -1;
        else if (desiredLane > 1) desiredLane = 1;
        else PlaySound(switchLaneSFX);
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
        controller.height /= 2f;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    void StopSliding()
    {
        animator.SetBool("Sliding", false);
        controller.height *= 2f;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
        if (audioSource.isPlaying && audioSource.clip == slideSFX) audioSource.Stop();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            isRunning = false;
            animator.SetTrigger("Death");
            GameManager.Instance.OnDeath();
        }
    }
}
