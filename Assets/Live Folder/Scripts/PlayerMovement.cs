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
    [SerializeField]
    private PlayerShooting playerShooting = null;

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

    [SerializeField]
    private float reduceYSpeedOnReleaseJumpInputMultiplier = 0.0f;
    [SerializeField]
    private float ySpeedOnStartJumpMultiplier = 0.0f;

    private bool isOnGround;
    private bool shouldJump;
    private bool shouldDoTriggerJump;
    private JumpState jumpState;
    private List<Collider2D> walls = new List<Collider2D>();
    private bool movementEnabled = true;

    [Header("Animations")]
    [SerializeField]
    private Animator playerAnimator = null;

    private TransformWrapper transformWrapper;

    private JumpStateClass currentJumpStateClass;

    private static PlayerMovement instance;
    public static readonly string TAG = "Player";

    public static Vector3 Position { get => Instance.transformWrapper.Position; }
    public static PlayerMovement Instance { get => instance; }
    public bool MovementEnabled 
    {
        set 
        {
            rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
            movementEnabled = value;
            playerShooting.enabled = value;
        }
    }

    // NOTE: Uncomment for testing if it's useful;
    // [SerializeField]
    // private SpriteRenderer spriteRenderer = null;

    private void Awake()
    {
        // NOTE: this shouln't be in this script. When some other script like GameManager or similar is created
        // please take the next line to that new script.
        Application.targetFrameRate = 60;

        instance = this;

        transformWrapper = new TransformWrapper(transform);

        currentJumpStateClass = new NotJumping();
    }

    private void Update()
    {
        if (movementEnabled)
        {
            this.shouldJump = shouldJump || (playerInput.GetJumpButtonDown() && isOnGround);
            
        }

        // Send 0.0f to "horizontal" so it animates like idle if movement is disabled
        playerAnimator.SetFloat("horizontal", movementEnabled?playerInput.HorizontalInput:0.0f);
        playerAnimator.SetBool("Fire1", playerInput.IsPressingShootInput && movementEnabled);
        playerAnimator.SetBool("isOnGround", isOnGround);
    }

    private void FixedUpdate()
    {
        // Is on ground
        {
            this.isOnGround = Physics2D.Raycast(bottomRaycastOrigin.transform.position, -Vector2.up, distance: 0.1f,
                               layerMask: (LayerMask.NameToLayer("Player") | /*Ignore Raycast*/ (1 << 2)
                               | LayerMask.NameToLayer("JumpTrigger") | LayerMask.NameToLayer("InvisibleStuff")));
        }

        // Movement
        if (movementEnabled)
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

                if (/*(!playerInput.IsPressingShootInput || !isOnGround) &&*/
                    (horizontalInput > 0.0f && canIGoRight || horizontalInput < 0.0f && canIGoLeft))
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

            // Jumping
            {
                JumpStateClass nextJumpStateClass = currentJumpStateClass.GetNextState(this);

                if (nextJumpStateClass != currentJumpStateClass)
                {
                    nextJumpStateClass.Initialize(this);
                }

                currentJumpStateClass = nextJumpStateClass;
            }
        }
    }
    private abstract class JumpStateClass
    {
        public abstract void Initialize(PlayerMovement playerMovement);
        public abstract JumpStateClass GetNextState(PlayerMovement playerMovement);
    }
    private class NotJumping : JumpStateClass
    {
        public override void Initialize(PlayerMovement playerMovement)
        {
            playerMovement.rb2d.gravityScale = 4.0f;
        }
        public override JumpStateClass GetNextState(PlayerMovement playerMovement)
        {
            JumpStateClass nextState = this;

            if (playerMovement.shouldDoTriggerJump)
            {
                nextState = new StartTriggerJumpClass();
            }
            if (playerMovement.shouldJump)
            {
                nextState = new BeforeReleasingJumpButton(shouldApplyForceOnInitialize: true);
            }

            return nextState;
        }
    }
    private class BeforeReleasingJumpButton : JumpStateClass
    {
        private bool shouldApplyForceOnInitialize;
        private bool leftTheGround;

        public BeforeReleasingJumpButton(bool shouldApplyForceOnInitialize)
        {
            this.shouldApplyForceOnInitialize = shouldApplyForceOnInitialize;
        }
        public override void Initialize(PlayerMovement playerMovement)
        {
            playerMovement.shouldJump = false;
            if (shouldApplyForceOnInitialize)
            {
                // Multiply Y velocity as it's not ideal to sum the current velocity
                // neither to reduce it to zero before jumping.
                Vector2 velAux = playerMovement.rb2d.velocity;
                velAux.y = velAux.y * playerMovement.ySpeedOnStartJumpMultiplier;
                playerMovement.rb2d.velocity = velAux;

                playerMovement.rb2d.AddForce(Vector2.up * playerMovement.initialJumpSpeed, 
                                                ForceMode2D.Impulse);
            }
            playerMovement.rb2d.gravityScale = 2.0f;
        }
        public override JumpStateClass GetNextState(PlayerMovement playerMovement)
        {
            // NOTE: leftTheGround management should be in an Update() method, but,
            // only this is not enugh to implement Update method in all states
            if (!playerMovement.isOnGround)
            {
                leftTheGround = true;
            }

            JumpStateClass nextState = this;

            if (playerMovement.shouldDoTriggerJump)
            {
                nextState = new StartTriggerJumpClass();
            }
            else if (!playerMovement.playerInput.IsPressingJumpInput)
            {
                nextState = new AfterReleasingJumpButton();
            }
            // TODO: test without the velocity check
            else if (playerMovement.isOnGround && leftTheGround)
            {
                nextState = new NotJumping();
            }

            return nextState;
        }
    }
    private class AfterReleasingJumpButton : JumpStateClass
    {
        public AfterReleasingJumpButton()
        {
        }
        public override void Initialize(PlayerMovement playerMovement)
        {
            playerMovement.rb2d.gravityScale = 4.0f;

            Rigidbody2D rb2d = playerMovement.rb2d;
            if (rb2d.velocity.y > 0.0f)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x,
                    playerMovement.reduceYSpeedOnReleaseJumpInputMultiplier * rb2d.velocity.y);
            }
        }
        public override JumpStateClass GetNextState(PlayerMovement playerMovement)
        {
            JumpStateClass nextJumpState = this;

            if (playerMovement.shouldDoTriggerJump)
            {
                nextJumpState = new StartTriggerJumpClass();
            }
            else if (playerMovement.isOnGround)
            {
                nextJumpState = new NotJumping();
            }

            return nextJumpState;
        }
    }
    private class StartTriggerJumpClass : JumpStateClass
    {
        private bool leftTheGround;

        public override void Initialize(PlayerMovement playerMovement)
        {
            playerMovement.rb2d.gravityScale = 3.5f;

            playerMovement.shouldDoTriggerJump = false;
        }
        public override JumpStateClass GetNextState(PlayerMovement playerMovement)
        {
            // NOTE: the next lines take advantage of the fact this method executes every frame
            // to set properlt the leftTheGround attribute
            // Update
            if (!playerMovement.isOnGround)
            {
                this.leftTheGround = true;
            }

            // GetNextState
            JumpStateClass nextState = this;

            if (playerMovement.playerInput.IsPressingJumpInput)
            {
                // Don't apply force as it should have been applied already
                // on playerMovement.ApplyJumpTriggerImpulse method
                nextState = new BeforeReleasingJumpButton(shouldApplyForceOnInitialize: false);
            }
            else if (leftTheGround && playerMovement.isOnGround)
            {
                nextState = new NotJumping();
            }

            return nextState;
        }
    }
    private class TriggerJumping : JumpStateClass
    {
        public override void Initialize(PlayerMovement playerMovement)
        {
            playerMovement.shouldJump = false;
            playerMovement.shouldDoTriggerJump = false;
            playerMovement.rb2d.gravityScale = 2.0f;
        }
        public override JumpStateClass GetNextState(PlayerMovement playerMovement)
        {
            JumpStateClass nextState = this;

            if (playerMovement.playerInput.IsPressingJumpInput)
            {
                // Don't apply impulse as it's already doing a jump
                nextState = new BeforeReleasingJumpButton(shouldApplyForceOnInitialize: false);
            }
            // TODO: test without the velocity check
            else if (playerMovement.isOnGround && playerMovement.rb2d.velocity.y > 0.0f)
            {
                nextState = new NotJumping();
            }

            return nextState;
        }
    }

    // Movement
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "scenario")
        {
            walls.Add(col);
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "scenario")
        {
            walls.Remove(col);
        }
    }
    public void ApplyJumpTriggerImpulse(Vector3 impulse)
    {
        shouldDoTriggerJump = true;

        // Set velocity to zero to get more stability and avoid 
        // the player to jump naturally in the same frame
        Vector2 vel = rb2d.velocity;
        vel.y = 0.0f;
        rb2d.velocity = vel;

        rb2d.AddForce(impulse, ForceMode2D.Impulse);
    }

    public float GetVelocityMagnitude()
    {
        return rb2d.velocity.magnitude;
    }

    public void SetParent(TransformWrapper parent)
    {
        transformWrapper.SetParent(parent);
    }
}
