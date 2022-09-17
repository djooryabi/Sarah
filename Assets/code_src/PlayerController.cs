using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public float MAX_FIRST_JUMP_TIME = 1f;
    public float MELEE_WEAPON_ACTIVE_TIME = 0.1f;
    public const int MAX_JUMP_COUNT = 2;
    public const float INPUT_DEAD_ZONE = 0.1f;

    public float maxXSpeed;
    public float maxYSpeed;

    private float inputX;
    private bool meleeKeyJustPressed;
    private bool jumpKeyJustPressed;
    private bool jumpKeyHeldDown;

    public float firstJumpTimer;
    public float firstJumpForce;
    public float firstJumpInProgressForce;
    public float doubleJumpForce;

    private Rigidbody2D rb;

    private int currentJumpCount;

    private bool isGrounded;
    private MeleeWeapon meleeWeapon;
    private TimedActionManager timedActionManager;
    private enum VerticalMovementState
    {
        Error,
        Standing,
        FirstJumpStart,
        FirstJumpInProgress,
        FirstJumpCompleted,
        DoubleJumpStart,
        DoubleJumpCompleted,
        Falling
    }

    private enum HorizontalMovementState
    {
        Error,
        Standing,
        RunningLeft,
        RunningRight
    }

    private VerticalMovementState verticalMovementState;
    private HorizontalMovementState horizontalMovementState;

    private const string MELEE_WEAPON_ACTIVE_TIME_KEY = "MELEE_WEAPON_ACTIVE_TIME_KEY";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.gameObject.SetActive(false);
        timedActionManager = new TimedActionManager();
    }

    private void IncrementFirstJumpTimer()
    {
        firstJumpTimer += Time.deltaTime;
    }

    private bool FirstJumpTimeExceeded()
    {
        return firstJumpTimer >= MAX_FIRST_JUMP_TIME;
    }

    private void ActivateMeleeWeapon()
    {
        meleeWeapon.gameObject.SetActive(true);
    }

    private void DeactivateMeleeWeapon()
    {
        meleeWeapon.gameObject.SetActive(false);
    }

    private HorizontalMovementState GetHorizontalMovementState()
    {
        HorizontalMovementState newState = HorizontalMovementState.Error;

        if (inputX > INPUT_DEAD_ZONE)
        {
            newState = HorizontalMovementState.RunningRight;
        }
        else if (inputX < -INPUT_DEAD_ZONE)
        {
            newState = HorizontalMovementState.RunningLeft;
        }
        else
        {
            newState = HorizontalMovementState.Standing;
        }

        return newState;
    }

    private VerticalMovementState GetVerticalMovementState()
    {
        VerticalMovementState newState = VerticalMovementState.Error;

        if (isGrounded)
        {
            if (jumpKeyJustPressed)
            {
                newState = VerticalMovementState.FirstJumpStart;
            }
            else
            {
                newState = VerticalMovementState.Standing;
            }
        }
        else
        {
            if (currentJumpCount < MAX_JUMP_COUNT)
            {
                if (rb.velocity.y > 0f)
                {
                    if (jumpKeyJustPressed)
                    {
                        newState = VerticalMovementState.DoubleJumpStart;
                    }
                    else if (jumpKeyHeldDown && !FirstJumpTimeExceeded())
                    {
                        newState = VerticalMovementState.FirstJumpInProgress;
                    }
                    else
                    {
                        newState = VerticalMovementState.FirstJumpCompleted;
                    }
                }
                else
                {
                    if (jumpKeyJustPressed)
                    {
                        newState = VerticalMovementState.DoubleJumpStart;
                    }
                    else
                    {
                        newState = VerticalMovementState.Falling;
                    }
                }
            }
            else
            {
                if (rb.velocity.y > 0f)
                {
                    newState = VerticalMovementState.DoubleJumpCompleted;
                }
                else
                {
                    newState = VerticalMovementState.Falling;
                }
            }

        }

        return newState;
    }

    private void MeleeAttack()
    {
        if (timedActionManager.ContainsTimedAction(MELEE_WEAPON_ACTIVE_TIME_KEY))
        {
            return;
        }

        ActivateMeleeWeapon();
        TimedAction deactivateMeleeWeaponAction = new TimedAction(DeactivateMeleeWeapon, MELEE_WEAPON_ACTIVE_TIME);
        timedActionManager.RegisterTimedAction(MELEE_WEAPON_ACTIVE_TIME_KEY, deactivateMeleeWeaponAction);
    }

    private void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyJustPressed = true;
        }

        jumpKeyHeldDown = Input.GetKey(KeyCode.Space);

        if (!meleeKeyJustPressed)
        {
            meleeKeyJustPressed = Input.GetKeyDown(KeyCode.E);
        }

        inputX = Input.GetAxisRaw("Horizontal");
    }

    void Update()
    {
        timedActionManager.Update(Time.deltaTime);
        HandlePlayerInput();
        verticalMovementState = GetVerticalMovementState();
        horizontalMovementState = GetHorizontalMovementState();
    }

    private void MovePlayer()
    {
        switch (horizontalMovementState)
        {
            case HorizontalMovementState.Standing:
                break;
            case HorizontalMovementState.RunningRight:
            case HorizontalMovementState.RunningLeft:
                rb.AddForce(new Vector2(inputX * maxXSpeed, 0f), ForceMode2D.Impulse);
                break;

        }

        switch (verticalMovementState)
        {
            case VerticalMovementState.Standing:
                firstJumpTimer = 0f;
                currentJumpCount = 0;
                break;

            case VerticalMovementState.FirstJumpStart:
                jumpKeyJustPressed = false;
                currentJumpCount++;
                rb.AddForce(new Vector2(0f, firstJumpForce), ForceMode2D.Impulse);
                break;
            case VerticalMovementState.FirstJumpInProgress:
                firstJumpTimer += Time.deltaTime;
                rb.AddForce(new Vector2(0f, firstJumpInProgressForce), ForceMode2D.Force);
                break;
            case VerticalMovementState.FirstJumpCompleted:
                jumpKeyHeldDown = false;
                break;

            case VerticalMovementState.DoubleJumpStart:
                jumpKeyJustPressed = false;
                currentJumpCount++;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(0f, rb.velocity.y) + doubleJumpForce);
                break;

            case VerticalMovementState.DoubleJumpCompleted:
                jumpKeyHeldDown = false;
                break;

            case VerticalMovementState.Falling:
                break;

            case VerticalMovementState.Error:
                break;
        }

        float currentXSpeed = rb.velocity.x;
        float currentYSpeed = rb.velocity.y;

        Vector2 clampedVelocity = rb.velocity;

        clampedVelocity.x = Mathf.Clamp(currentXSpeed, -maxXSpeed, maxXSpeed);
        //clampedVelocity.y = Mathf.Clamp(currentYSpeed, -maxYSpeed, maxYSpeed);

        rb.velocity = clampedVelocity;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void CheckIfPlatformTouched(Collider2D collision)
    {
        if (collision.gameObject.tag != "Platform")
        {
            return;
        }

        isGrounded = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckIfPlatformTouched(collision);
    }

    private void CheckIfPlatformExited(Collider2D collision)
    {
        if (collision.gameObject.tag != "Platform")
        {
            return;
        }

        isGrounded = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckIfPlatformExited(collision);
    }
}
