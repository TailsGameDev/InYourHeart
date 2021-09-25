using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum JumpState
    {
        NOT_JUMPING,
        BEFORE_RELEASING_JUMP_BUTTON,
        AFTER_RELEASING_JUMP_BUTTON,
        STARTING_TRIGGER_JUMP,
    }

    [SerializeField]
    private PlayerInput playerInput = null;

    [Header("Movement")]
    [SerializeField]
    private Rigidbody2D rb2d = null;
    [SerializeField]
    private Transform bottomRaycastOrigin = null;

    [SerializeField]
    private Transform flipXObject = null;
    [SerializeField]
    private float horizontalSpeed = 0.0f;
    [SerializeField]
    private float horizontalDrag = 0.0f;
    [SerializeField]
    private float initialJumpSpeed = 0.0f;

    private bool isOnGround;
    private bool shouldJump;
    private bool shouldDoTriggerJump;
    private JumpState jumpState;
    private List<Collider2D> walls = new List<Collider2D>();

    [Header("Shooting")]
    [SerializeField]
    private Transform arrowSpawner = null;
    [SerializeField]
    private Transform arrowPrefab = null;
    
    [SerializeField]
    private float maxChargingAttackImpulse = 0.0f;
    [SerializeField]
    private float chargingSpeed = 0.0f;

    private float currentCharge;

    [Header("Animations")]
    [SerializeField]
    private SpriteRenderer spriteRenderer = null;
    [SerializeField]
    private Animator playerAnimator = null;

    private void Awake()
    {
        // NOTE: this shouln't be in this script. When some other script like GameManager or similar is created
        // please take the next line to that new script.
        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        this.shouldJump = shouldJump || (Input.GetButtonDown("Jump") && isOnGround);

        // Shooting
        if (Input.GetButtonUp("Fire1"))
        {
            Instantiate(arrowPrefab, arrowSpawner.position, arrowSpawner.rotation)
                .GetComponent<Bullet>().ApplyImpulse(rb2d.velocity.magnitude + currentCharge);

            currentCharge = 0.0f;
        }

        // Animations
        playerAnimator.SetFloat("horizontal", playerInput.HorizontalInput);
        playerAnimator.SetBool("isOnGround", isOnGround);
        playerAnimator.SetBool("Fire1", playerInput.IsPressingShootInput);
    }

    private void FixedUpdate()
    {

        // Movement
        {
            // Horizontal Movement
            {
                float horizontalInput = playerInput.HorizontalInput;
                
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

                if ( (!playerInput.IsPressingShootInput || !isOnGround) && (horizontalInput > 0.0f && canIGoRight || horizontalInput < 0.0f && canIGoLeft))
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
                this.isOnGround = Physics2D.Raycast(bottomRaycastOrigin.transform.position, -Vector2.up, distance: 0.1f, 
                                   layerMask: (LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("JumpTrigger")));
            }

            // Jumping
            {
                bool isPressingJumpInput = playerInput.IsPressingJumpInput;

                // Output Logic
                // NOTE: We are writting on rb2d on every fixed frame. That's not optimized, but let solve this later so
                // code is more organized now.
                switch (jumpState)
                {
                    case JumpState.NOT_JUMPING:
                        rb2d.gravityScale = 5.0f;
                        break;
                    case JumpState.BEFORE_RELEASING_JUMP_BUTTON:
                        rb2d.gravityScale = 2.0f;
                        break;
                    case JumpState.AFTER_RELEASING_JUMP_BUTTON:
                        rb2d.gravityScale = 5.0f;
                        break;
                    case JumpState.STARTING_TRIGGER_JUMP:
                        rb2d.gravityScale = 3.5f;
                        break;
                }

                // Next State Logic and State Transition Actions
                switch (jumpState) 
                {
                    case JumpState.NOT_JUMPING:
                        if (this.shouldJump)
                        {
                            // Next State Logic
                            this.jumpState = JumpState.BEFORE_RELEASING_JUMP_BUTTON;
                        
                            // State Transition Actions
                            this.shouldJump = false;
                            rb2d.AddForce(Vector2.up * initialJumpSpeed);
                        }
                        break;
                    case JumpState.BEFORE_RELEASING_JUMP_BUTTON:
                        if (shouldDoTriggerJump)
                        {
                            // Next State Logic
                            this.jumpState = JumpState.STARTING_TRIGGER_JUMP;

                            // State Transition Actions
                            shouldDoTriggerJump = false;
                        }
                        else if (!isPressingJumpInput)
                        {
                            // Next State Logic
                            this.jumpState = JumpState.AFTER_RELEASING_JUMP_BUTTON;

                            // State Transition Actions
                            if (rb2d.velocity.y > 0.0f)
                            {
                                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
                            }
                        }
                        else if (isOnGround && rb2d.velocity.y > 0.0f)
                        {
                            // Next State Logic
                            this.jumpState = JumpState.NOT_JUMPING;
                        }
                        break;
                    case JumpState.AFTER_RELEASING_JUMP_BUTTON:
                        if (shouldDoTriggerJump)
                        {
                            // Next State Logic
                            this.jumpState = JumpState.STARTING_TRIGGER_JUMP;

                            // State Transition Actions
                            shouldDoTriggerJump = false;
                        }
                        else if (this.isOnGround)
                        {
                            // Next State Logic
                            this.jumpState = JumpState.NOT_JUMPING;
                        }
                        break;
                    case JumpState.STARTING_TRIGGER_JUMP:
                        // State Transition Actions
                        if (isPressingJumpInput)
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
        }

        // Shooting
        {
            if (playerInput.IsPressingShootInput)
            {
                currentCharge = Mathf.Clamp(currentCharge + (chargingSpeed* maxChargingAttackImpulse), 
                                            min: 0.0f, max: maxChargingAttackImpulse);
            }

            spriteRenderer.color = Color.Lerp(Color.white, Color.magenta, currentCharge/maxChargingAttackImpulse);
        }
    }

    // Movement
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