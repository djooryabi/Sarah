using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxXSpeed;
    public float maxYSpeed;

    private float inputX;
    private float inputY;

    public float jumpForce;

    private Rigidbody2D rb;

    private bool isGrounded;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (inputX > 0.1f || inputX < -0.1f)
        {
            rb.AddForce(new Vector2(inputX * maxYSpeed, 0f), ForceMode2D.Impulse);
        }

        if (inputY > 0.1f && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        float currentXSpeed = rb.velocity.x;
        float currentYSpeed = rb.velocity.y;

        Vector2 clampedVelocity = rb.velocity;

        clampedVelocity.x = Mathf.Clamp(currentXSpeed, -maxXSpeed, maxXSpeed);
        clampedVelocity.y = Mathf.Clamp(currentYSpeed, -maxYSpeed, maxYSpeed);

        rb.velocity = clampedVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = false;
        }
    }
}
