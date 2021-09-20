using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum JumpState
    {
        NOT_JUMPING,
        BEFORE_RELEASING_JUMP_BUTTON,
        AFTER_RELEASING_JUMP_BUTTON,
        STARTING_TRIGGER_JUMP,
    }

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

    [SerializeField]
    private Animator playerAnimator = null;

    [SerializeField]
    private Transform flipXObject = null;

    [SerializeField]
    private Transform arrowSpawner = null;

    [SerializeField]
    private Transform arrowPrefab = null;

    [SerializeField]
    private float maxChargingAttackImpulse = 0.0f;
    [SerializeField]
    private float chargingSpeed = 0.0f;

    private float currentCharge;

    private float horizontalInput;

    private bool isPressingShootInput;
    private bool isPressingJumpInput;
    private bool shouldDoTriggerJump;

    private bool shouldJump;
    private bool isOnGround;

    private JumpState jumpState;

    private List<Collider2D> walls = new List<Collider2D>();

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        this.horizontalInput = Input.GetAxisRaw("Horizontal");

        this.isPressingJumpInput = Input.GetButton("Jump");
        this.shouldJump = shouldJump || ( Input.GetButtonDown("Jump") && isOnGround );

        this.isPressingShootInput = Input.GetButton("Fire1");

        // Animations
        playerAnimator.SetFloat("horizontal", horizontalInput);
        playerAnimator.SetBool("isOnGround", isOnGround);
        playerAnimator.SetBool("Fire1", isPressingShootInput);

        // Shoot
        if (Input.GetButtonUp("Fire1"))
        {
            Instantiate(arrowPrefab, arrowSpawner.position, arrowSpawner.rotation)
                .GetComponent<Bullet>().ApplyImpulse(rb2d.velocity.magnitude + currentCharge);

            currentCharge = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        // Horizontal
        {
            bool canIGoRight = true;
            foreach (Collider2D wall in walls)
            {
                canIGoRight = canIGoRight && wall.transform.position.x < transform.position.x;
            }

            bool canIGoLeft = true;
            foreach (Collider2D wall in walls)
            {
                canIGoLeft = canIGoLeft && wall.transform.position.x > transform.position.x;
            }

            if ( (!isPressingShootInput || !isOnGround) && (horizontalInput > 0.0f && canIGoRight || horizontalInput < 0.0f && canIGoLeft))
            {
                rb2d.AddForce(horizontalInput * horizontalSpeed * Vector2.right);
            }

            // Flip X Object (Rotation)
            Vector3 anglesForFlipXObject = flipXObject.transform.eulerAngles;
            if (horizontalInput > 0.1f)
            {
                anglesForFlipXObject.y = 0.0f;
            }
            else if (horizontalInput < -0.1f)
            {
                anglesForFlipXObject.y = 180.0f;
            }
            flipXObject.transform.eulerAngles = anglesForFlipXObject;

            // Drag
            rb2d.AddForce( horizontalDrag * rb2d.velocity.x * - Vector2.right);
        }

        // Is on ground
        {
            // this.isOnGround = Physics2D.Raycast(bottomRaycastOrigin.transform.position, -Vector2.up, distance: 0.1f,
            //                     layerMask: (LayerMask.NameToLayer("Player")));

            this.isOnGround = Physics2D.Raycast(bottomRaycastOrigin.transform.position, -Vector2.up, distance: 0.1f, 
                               layerMask: (LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("JumpTrigger")));
            
            /* PAINT THE PLAYER RED IF IT IS ON GROUND
            this.isOnGround = Physics2D.BoxCast(origin: bottomRaycastOrigin.transform.position, size: new Vector2(0.35f, 0.2f), 
                angle: 0.0f, direction: - Vector2.up, distance: 0.1f, layerMask: LayerMask.NameToLayer("Player"));
            if (isOnGround)
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
            */
        }

        // Jump
        {
            // Output Logic
            switch (jumpState)
            {
                case JumpState.NOT_JUMPING:
                    rb2d.gravityScale = 5.0f;
                    // spriteRenderer.color = Color.white;
                    break;
                case JumpState.BEFORE_RELEASING_JUMP_BUTTON:
                    rb2d.gravityScale = 2.0f;
                    // spriteRenderer.color = Color.red;
                    break;
                case JumpState.AFTER_RELEASING_JUMP_BUTTON:
                    rb2d.gravityScale = 5.0f;
                    // spriteRenderer.color = Color.blue;
                    break;
                case JumpState.STARTING_TRIGGER_JUMP:
                    rb2d.gravityScale = 3.5f;
                    // spriteRenderer.color = Color.yellow;
                    break;
            }

            // Next State Logic and State Transition Actions
            switch (jumpState) 
            {
                case JumpState.NOT_JUMPING:
                    if (this.shouldJump)
                    {
                        this.shouldJump = false;
                        this.jumpState = JumpState.BEFORE_RELEASING_JUMP_BUTTON;
                        
                        rb2d.AddForce(Vector2.up * initialJumpSpeed);
                    }
                    break;
                case JumpState.BEFORE_RELEASING_JUMP_BUTTON:
                    if (shouldDoTriggerJump)
                    {
                        this.jumpState = JumpState.STARTING_TRIGGER_JUMP;
                        shouldDoTriggerJump = false;
                    }
                    else if (!this.isPressingJumpInput)
                    {
                        this.jumpState = JumpState.AFTER_RELEASING_JUMP_BUTTON;

                        if (rb2d.velocity.y > 0.0f)
                        {
                            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
                        }
                    }
                    else if (isOnGround && rb2d.velocity.y > 0.0f)
                    {
                        this.jumpState = JumpState.NOT_JUMPING;
                    }
                    break;
                case JumpState.AFTER_RELEASING_JUMP_BUTTON:
                    if (shouldDoTriggerJump)
                    {
                        this.jumpState = JumpState.STARTING_TRIGGER_JUMP;
                        shouldDoTriggerJump = false;
                    }
                    else if (this.isOnGround)
                    {
                        this.jumpState = JumpState.NOT_JUMPING;
                    }
                    break;
                case JumpState.STARTING_TRIGGER_JUMP:
                    if (this.isPressingJumpInput)
                    {
                        this.jumpState = JumpState.BEFORE_RELEASING_JUMP_BUTTON;

                    }
                    else if (this.isOnGround)
                    {
                        this.jumpState = JumpState.NOT_JUMPING;
                    }
                    break;
            }
        }

        // Charging Attack
        {
            if (isPressingShootInput)
            {
                currentCharge = Mathf.Clamp(currentCharge + (chargingSpeed* maxChargingAttackImpulse), 
                                            min: 0.0f, max: maxChargingAttackImpulse);
            }

            spriteRenderer.color = Color.Lerp(Color.white, Color.magenta, currentCharge/maxChargingAttackImpulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "wall")
        {
            walls.Add(col);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "wall")
        {
            walls.Remove(col);
        }
    }

    public void ApplyJumpTriggerImpulse(Vector3 impulse)
    {
        shouldDoTriggerJump = true;
        rb2d.AddForce(impulse, ForceMode2D.Impulse);
    }
}
