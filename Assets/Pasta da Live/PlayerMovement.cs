using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb2d = null;

    [SerializeField]
    private float horizontalSpeed = 0.0f;

    [SerializeField]
    private float horizontalDrag = 0.0f;

    [SerializeField]
    private float initialJumpSpeed = 0.0f;

    [SerializeField]
    private Transform bottomRaycastOrigin = null;

    [SerializeField]
    private SpriteRenderer spriteRenderer = null;

    private float horizontalInput;

    private bool jumpInput;
    private bool isOnGround;

    private void Update()
    {
        this.horizontalInput = Input.GetAxisRaw("Horizontal");

        bool localJumpInput = Input.GetButtonDown("Jump");

        if (this.jumpInput)
        {

        }
        else
        {
            if (localJumpInput && isOnGround)
            {
                rb2d.AddForce(Vector2.up * initialJumpSpeed);
            }
        }

        this.jumpInput = localJumpInput;
    }

    private void FixedUpdate()
    {
        // Horizontal
        {
            rb2d.AddForce(horizontalInput * horizontalSpeed * Vector2.right);

            // Drag
            rb2d.AddForce( horizontalDrag * rb2d.velocity.x * - Vector2.right);
        }

        // Is on ground
        {
            this.isOnGround = Physics2D.Raycast(bottomRaycastOrigin.transform.position, -Vector2.up, distance: 0.1f);
            if (isOnGround)
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}
