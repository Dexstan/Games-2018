using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DexCode
{
    public class FinitePlayerLogic : StateMachine
    {
        #region Holder Values

        [SerializeField] float mouseOffsetX;

        #region Raycast Holders

        [Header("Raycast Checks")]
        [SerializeField] Transform rayLeft, rayRight, rayDown; // TODO: Look into this
        [SerializeField] float rayRadius;
        [SerializeField] float slopeCheckDistance;
        [SerializeField] LayerMask groundMask;

        #endregion

        #region Slope Holders
        RaycastHit2D slopeHitFront;
        RaycastHit2D slopeHitBack;
        RaycastHit2D hit;

        private float slopeDownAngle;
        private float slopeDownAngleOld;
        private float slopeSideAngle;
        [SerializeField] float maxSlopeAngle;

        private Vector2 slopeNormalPerp;
        private Vector2 checkPos;

        public bool canWalkOnSlope { get; private set; }
        [SerializeField] public bool isOnSlope { get; private set; }
        #endregion       

        #region GameInfo
        private Vector3 characterScale;
        private Vector2 colliderSize;
        private Transform graphicsObject;
        private GameObject currentInteractableObject; // TODO change

        public bool lookingRight { get; private set; }
        public bool isGrounded { get; private set; }
        #endregion

        #endregion

        #region Player Components
        [SerializeField] private FinitePlayerData playerData;
        public PlayerInputHandler playerInputHandler { get; private set; }
        public Animator Anim { get; private set; }
        public Rigidbody2D rb2d { get; private set; }
        public CapsuleCollider2D myCollider;

        [SerializeField] private PhysicsMaterial2D fullFriction;
        [SerializeField] private PhysicsMaterial2D noFriction; 
        public Vector2 currentVelocity { get; private set; }

        #endregion

        #region Player States

        public PlayerIdleState playerIdle;
        public PlayerMoveState playerMove;
        public PlayerBackWardsMovement playerBmovement;

        public PlayerDuckState playerDuck;
        public PlayerDuckMoveState playerDuckMove;
        public PlayerBackwardsDuckMovementState playerBDuckMovment;

        public PlayerRunningState playerRunning;
        public PlayerSlideState playerSlide;
        public PlayerJumpState playerJump;
        public FallingState playerAirborn;
        public PlayerLandingState playerLanding;

        #endregion

        #region Unity Stream
        private void Awake()
        {
            #region Initialize States

            playerIdle = new PlayerIdleState(this, playerData, "idle");
            playerMove = new PlayerMoveState(this, playerData, "move");
            playerDuck = new PlayerDuckState(this, playerData, "duck");
            playerDuckMove = new PlayerDuckMoveState(this, playerData, "duckmove");
            playerSlide = new PlayerSlideState(this, playerData, "slide");
            playerJump = new PlayerJumpState(this, playerData, "jump");
            playerAirborn = new FallingState(this, playerData, "airborn");
            playerLanding = new PlayerLandingState(this, playerData, "landing");
            playerBmovement = new PlayerBackWardsMovement(this, playerData, "backwardsmove");
            playerBDuckMovment = new PlayerBackwardsDuckMovementState(this, playerData, "duckbackwardsmove");
            playerRunning = new PlayerRunningState(this, playerData, "isrunning");

            #endregion

            #region Initilize Components
            graphicsObject = transform.GetChild(0).transform.GetChild(0);
            rb2d = GetComponent<Rigidbody2D>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
            Anim = GetComponent<Animator>();
            myCollider = GetComponent<CapsuleCollider2D>();
            colliderSize = myCollider.size;
            #endregion
        }

        private void Start()
        {
            Initilize(playerIdle);
        }

        private void Update()
        {
            currentVelocity = rb2d.velocity;
            CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            CurrentState.PhysicsUpdate();
        }

        #endregion

        #region Logic Checks

        #region SlopeChecks
        public void SlopeCheck()
        {
            checkPos = transform.position - new Vector3(0f, colliderSize.y / 2);
            SlopeCheckVertical(checkPos);
            SlopeCheckHorizontal(checkPos);
        }

        public void SlopeCheckHorizontal(Vector2 checkPos)
        {
            slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundMask);
            slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundMask);

            if (slopeHitFront)
            {
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            } else if (slopeHitBack)
            {
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            }
            else
            {
                slopeSideAngle = 0f;
                isOnSlope = false;
            }

        }

        public void SlopeCheckVertical(Vector2 checkPos)
        {
            hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundMask);

            if (hit)
            {
                slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeDownAngle != slopeDownAngleOld)
                {
                    isOnSlope = true;
                }

                slopeDownAngleOld = slopeDownAngle;

                //Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
                //Debug.DrawRay(hit.point, hit.normal,Color.green);
            }

            if(slopeDownAngle > maxSlopeAngle)
            {
                canWalkOnSlope = false;
            } else
            {
                canWalkOnSlope = true;
            }


            if(isOnSlope && playerInputHandler.MovementInput.x == 0 && canWalkOnSlope)
            {
                rb2d.sharedMaterial = fullFriction;
            } else
            {
                rb2d.sharedMaterial = noFriction;
            }

        }
        //Look at this
        public void SetPlayerFritcion(PhysicsMaterial2D frictionMat)
        {
            rb2d.sharedMaterial = frictionMat;
        }
        #endregion

        public void GroundCheck()
        {
            isGrounded = Physics2D.CircleCast(rayDown.transform.position, rayRadius, Vector2.zero, rayRadius, groundMask);
        }

        public void PlayerGravityModifier()
        {
            if (!isGrounded)
            {
                if (rb2d.velocity.y > playerData.maxJumpVelocty)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x,playerData.maxJumpVelocty);
                }
                if (rb2d.velocity.y < 0)
                {
                    rb2d.velocity += Vector2.up * Physics2D.gravity.y * (playerData.fallMultiplier) * Time.deltaTime;
                }
                else if (rb2d.velocity.y > 0)
                {
                    rb2d.velocity += Vector2.up * Physics2D.gravity.y * (playerData.risingGravity) * Time.deltaTime;
                }
            }
        }

        #endregion

        #region Actions
        public void PlayerMovement(float speed, Vector2 input)
        {
             if(!isOnSlope)
             {
                 rb2d.velocity = new Vector2(input.x * speed * Time.deltaTime, currentVelocity.y);
             }
             else if (isOnSlope && canWalkOnSlope)
             {
                 rb2d.velocity = new Vector2(-input.x * speed * Time.deltaTime * slopeNormalPerp.x, speed * slopeNormalPerp.y * -input.x * Time.deltaTime);
             }
        }

        public void PlayerSlide(float speed)
        {
            rb2d.AddForce(currentVelocity.x * Vector2.right * speed * Time.deltaTime , ForceMode2D.Impulse);
        }

        public void PlayerJump(float jumpForce)
        {
            rb2d.velocity = currentVelocity;
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        public void PlayerAirMovement(Vector2 moveInput,float airSpeed)
        {
            rb2d.AddForce((Vector2.right * moveInput.x) * airSpeed * Time.fixedDeltaTime,ForceMode2D.Force);
        }
        #endregion

        #region Animation Functions
        public void FlipPlayer()
        {
            characterScale = graphicsObject.localScale;
            if (playerInputHandler.CurrentMousePostion.x > transform.position.x + mouseOffsetX)
            {
                characterScale.x = 1;
                lookingRight = true;
            }
            else if (playerInputHandler.CurrentMousePostion.x < transform.position.x - mouseOffsetX)
            {
                characterScale.x = -1;
                lookingRight = false;
            }
            graphicsObject.localScale = characterScale;
        }

        public void FlipPlayerOnMovement()
        {
            characterScale = graphicsObject.localScale;
            if (playerInputHandler.NormalizedMoveInput.x > 0)
            {
                characterScale.x = 1;
                lookingRight = true;
            }
            else if (playerInputHandler.NormalizedMoveInput.x < 0)
            {
                characterScale.x = -1;
                lookingRight = false;
            }
            graphicsObject.localScale = characterScale;
        }
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(rayDown.transform.position, rayRadius);
        }


        // TODO: Change system below

        public void PlayerInteract()
        {
            if (currentInteractableObject == null) return;
            currentInteractableObject.GetComponent<IInteractable>().Interact();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Interactable"))
            {
                currentInteractableObject = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Interactable"))
            {
                currentInteractableObject = null;
            }
        }

    }
}
