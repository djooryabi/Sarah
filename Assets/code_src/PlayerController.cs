using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public float MELEE_WEAPON_ACTIVE_TIME = 0.1f;
    public const int MAX_JUMP_COUNT = 2;
    public const float INPUT_DEAD_ZONE = 0.1f;

    public float maxXSpeed;
    public float maxYSpeed;

    private float inputX;
    private bool inputY;

    public float jumpForce;

    private Rigidbody2D rb;

    private int jumpCount;

    private bool isGrounded;
    private MeleeWeapon meleeWeapon;
    private TimedActionManager timedActionManager;

    private const string MELEE_WEAPON_ACTIVE_TIME_KEY = "MELEE_WEAPON_ACTIVE_TIME_KEY";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.gameObject.SetActive(false);
        timedActionManager = new TimedActionManager();
    }

    private void ActivateMeleeWeapon()
    {
        meleeWeapon.gameObject.SetActive(true);
    }

    private void DeactivateMeleeWeapon()
    {
        meleeWeapon.gameObject.SetActive(false);
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
        inputX = Input.GetAxisRaw("Horizontal");

        if (jumpCount < MAX_JUMP_COUNT && Input.GetKeyDown(KeyCode.Space))
        {
            inputY = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MeleeAttack();
        }
    }

    void Update()
    {
        timedActionManager.Update(Time.deltaTime);
        HandlePlayerInput();
    }

    private void MovePlayer()
    {
        if (inputX > INPUT_DEAD_ZONE || inputX < -INPUT_DEAD_ZONE)
        {
            rb.AddForce(new Vector2(inputX * maxYSpeed, 0f), ForceMode2D.Impulse);
        }

        if (inputY)
        {
            jumpCount++;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            inputY = false;
        }

        float currentXSpeed = rb.velocity.x;
        float currentYSpeed = rb.velocity.y;

        Vector2 clampedVelocity = rb.velocity;

        clampedVelocity.x = Mathf.Clamp(currentXSpeed, -maxXSpeed, maxXSpeed);
        clampedVelocity.y = Mathf.Clamp(currentYSpeed, -maxYSpeed, maxYSpeed);

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
        jumpCount = 0;
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
