using AndrewDowsett.CommonObservers;
using System.Collections.Generic;
using UnityEngine;

namespace AndrewDowsett.MovementSystems
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementController2D : MonoBehaviour, IUpdateObserver, IFixedUpdateObserver
    {
        [Header("Movement Data")]
        /// <summary>
        /// This is the movement data that will be used for this motor.
        /// </summary>
        [SerializeField] protected PlayerData data;

        [Space(20)]
        [Header("References")]
        /// <summary>
        /// The RigidBody2D that will be used to move the character and apply gravity to.
        /// </summary>
        [SerializeField] protected Rigidbody2D rb;
        /// <summary>
        /// This is the tranform that will be used to detect the ground. (Use the transforms scale to change the size of the box)
        /// </summary>
        [SerializeField] protected Transform groundCheck;
        /// <summary>
        /// This is the tranform that will be used to detect a wall in front of the character. (Use the transforms scale to change the size of the box)
        /// </summary>
        [SerializeField] protected Transform frontWallCheck;
        /// <summary>
        /// This is the tranform that will be used to detect a wall behind the character. (Use the transforms scale to change the size of the box)
        /// </summary>
        [SerializeField] protected Transform backWallCheck;
        /// <summary>
        /// This is the tranform that holds all of the Visuals for the character. Put all visuals as children of the Visual Gameobject. (Mesh Renderer, Capsule, Addons like cool glasses, etc.)
        /// </summary>
        [SerializeField] protected Transform visual;
        /// <summary>
        /// The layer that will be used by the groundCheck to detect the ground.
        /// </summary>
        [SerializeField] private LayerMask groundLayer;

        [Space(20)]
        [Header("Ability References")]
        /// <summary>
        /// This is an array of MotorMods that will be used by this motor.
        /// </summary>
        [SerializeField] private List<MotorMod> motorMods = new List<MotorMod>();


        /// <summary>
        /// Stores the input from the player.
        /// </summary>
        private Vector2 movementInput;
        /// <summary>
        /// Tells the Motor if it should update.
        /// </summary>
        private bool motorEnabled = true;
        /// <summary>
        /// Enables or disables the players input for the Motor.
        /// </summary>
        private bool inputEnabled = true;

        #region Wall Check Variables
        private Vector2 _wallCheckSize => new Vector2(0.1f, 0.1f);
        private Transform _frontWallCheckPoint => isFacingRight ? frontWallCheck : backWallCheck;
        private Transform _backWallCheckPoint => isFacingRight ? backWallCheck : frontWallCheck;
        #endregion

        #region Facing Direction Variables
        private bool isFacingRight;
        #endregion

        #region Jumping Variables
        public bool IsJumping => isJumping;
        public bool IsJumpFalling => isJumpFalling;
        public bool IsGrounded => isGrounded;
        public bool IsAbleToLand => lastOnGroundTime <= -data.timeBeforeLandingCheck;

        private bool isJumping;
        private bool isWallJumping;
        private bool isJumpFalling;
        private bool isJumpCut;
        private bool isGrounded;
        private float lastPressedJumpTime;
        private float lastOnGroundTime;
        private float lastOnWallTime;
        private float lastOnWallLeftTime;
        private float lastOnWallRightTime;
        private float wallJumpStartTime;
        private int lastWallJumpDir;
        #endregion

        private void Start()
        {
            UpdateManager.RegisterObserver(this);
            FixedUpdateManager.RegisterObserver(this);
            data.Init();
            HandleModStart();
        }

        public void ObservedUpdate(float deltaTime)
        {
            if (motorEnabled)
            {
                HandleTimers(deltaTime);
                SetMovementInput();
                HandleAcceleration();
                HandleDeceleration();
                HandleGroundCheck();
                HandleCharacterVisualMovement();
                HandleLookDirection();
                HandleJumpButtonDown();
                HandleJumpButtonUp();
                HandleCoyoteTime();
                HandleJumping();
                HandleGravity();
            }
            HandleModUpdate(deltaTime);
        }

        public void ObservedFixedUpdate(float fixedDeltaTime)
        {
            //Handle Run
            if (isWallJumping)
                Run(data.wallJumpRunLerp);
            else
                Run(1);
        }

        public void DisableMotor()
        {
            SetMovementInput(reset: true);
            motorEnabled = false;
        }
        public void EnableMotor()
        {
            motorEnabled = true;
        }

        public void DisableInputMovement()
        {
            SetMovementInput(reset: true);
            inputEnabled = false;
        }
        public void EnableInputMovement()
        {
            inputEnabled = true;
        }

        private void Run(float lerpAmount)
        {
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = movementInput.x * data.runMaxSpeed;
            //We can reduce are control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, lerpAmount);

            #region Calculate AccelRate
            float accelRate;

            //Gets an acceleration value based on if we are accelerating (includes turning) 
            //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            if (lastOnGroundTime > 0)
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelerationAmount : data.runDeccelerationAmount;
            else
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelerationAmount * data.accelerationInAir : data.runDeccelerationAmount * data.deccelerationInAir;
            #endregion

            #region Add Bonus Jump Apex Acceleration
            //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
            if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.linearVelocity.y) < data.jumpHangTimeThreshold)
            {
                accelRate *= data.jumpHangAccelerationMultiplier;
                targetSpeed *= data.jumpHangMaxSpeedMultiplier;
            }
            #endregion

            #region Conserve Momentum
            //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
            if (data.doConserveMomentum && Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
            {
                //Prevent any deceleration from happening, or in other words conserve are current momentum
                //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
                accelRate = 0;
            }
            #endregion

            //Calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - rb.linearVelocity.x;

            //Calculate force along x-axis to apply to the player
            float movement = speedDif * accelRate;

            //Convert this to a vector and apply to rigidbody
            rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }

        public void Jump()
        {
            //Set to tell anything listening that this is now off the ground.
            isGrounded = false;

            //Ensures we can't call Jump multiple times from one press
            lastPressedJumpTime = 0;
            lastOnGroundTime = 0;

            //We increase the force applied if we are falling
            //This means we'll always feel like we jump the same amount 
            float force = data.jumpForce;
            if (rb.linearVelocity.y < 0)
                force -= rb.linearVelocity.y;

            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        private void WallJump(int dir)
        {
            if (!data.doTurnOnWallJump)
                return;

            //Ensures we can't call Wall Jump multiple times from one press
            lastPressedJumpTime = 0;
            lastOnGroundTime = 0;
            lastOnWallRightTime = 0;
            lastOnWallLeftTime = 0;

            Vector2 force = new Vector2(data.wallJumpForce.x, data.wallJumpForce.y);
            force.x *= -dir; //apply force in opposite direction of wall

            if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(force.x))
                force.x -= rb.linearVelocity.x;

            if (rb.linearVelocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
                force.y -= rb.linearVelocity.y;

            //Unlike in the run we want to use the Impulse mode.
            //The default mode will apply are force instantly ignoring masss
            rb.AddForce(force, ForceMode2D.Impulse);
        }

        private void SetMovementInput(bool reset = false)
        {
            if (reset)
            {
                movementInput = Vector2.zero;
            }
            else if (inputEnabled)
            {
                movementInput.x = Input.GetAxisRaw("Horizontal");
                movementInput.y = Input.GetAxisRaw("Vertical");
            }
            else
            {
                movementInput = Vector2.zero;
            }
        }

        private void HandleModStart()
        {
            foreach (var mod in motorMods)
            {
                mod.OnStart(this);
            }
        }

        private void HandleModUpdate(float deltaTime)
        {
            foreach (var mod in motorMods)
            {
                mod.OnUpdate(deltaTime);
            }
        }

        private void HandleGroundCheck()
        {
            isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheck.localScale, 0, groundLayer);

            if (IsGrounded && IsAbleToLand)
                HandleLanding();
        }

        private void HandleLanding()
        {

        }

        private void HandleTimers(float deltaTime)
        {
            lastOnGroundTime -= deltaTime;
            lastOnWallTime -= deltaTime;
            lastOnWallRightTime -= deltaTime;
            lastOnWallLeftTime -= deltaTime;
            lastPressedJumpTime -= deltaTime;
        }

        private void HandleGravity()
        {
            if (rb.linearVelocity.y < 0 && movementInput.y < 0)
            {
                //Much higher gravity if holding down
                SetGravityScale(data.gravityScale * data.fastFallGravityMultiplier);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -data.maxFastFallSpeed));
            }
            else if (isJumpCut)
            {
                //Higher gravity if jump button released
                SetGravityScale(data.gravityScale * data.jumpCutGravityMultiplier);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -data.maxFallSpeed));
            }
            else if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.linearVelocity.y) < data.jumpHangTimeThreshold)
            {
                SetGravityScale(data.gravityScale * data.jumpHangGravityMultiplier);
            }
            else if (rb.linearVelocity.y < 0)
            {
                //Higher gravity if falling
                SetGravityScale(data.gravityScale * data.fallGravityMultiplier);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -data.maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                SetGravityScale(data.gravityScale);
            }
        }

        private void HandleJumping()
        {
            if (isJumping && rb.linearVelocity.y < 0)
            {
                isJumping = false;

                if (!isWallJumping)
                {
                    isJumpFalling = true;
                }
            }

            if (isWallJumping && Time.time - wallJumpStartTime > data.wallJumpTime)
            {
                isWallJumping = false;
            }

            if (lastOnGroundTime > 0 && !isJumping && !isWallJumping)
            {
                isJumpCut = false;

                if (!isJumping)
                {
                    isJumpFalling = false;
                }
            }

            //Jump
            if (CanJump() && lastPressedJumpTime > 0)
            {
                isJumping = true;
                isWallJumping = false;
                isJumpCut = false;
                isJumpFalling = false;
                Jump();
            }
            //Wall Jump
            else if (CanWallJump() && lastPressedJumpTime > 0)
            {
                isWallJumping = true;
                isJumping = false;
                isJumpCut = false;
                isJumpFalling = false;
                wallJumpStartTime = Time.time;
                lastWallJumpDir = (lastOnWallRightTime > 0) ? -1 : 1;

                WallJump(lastWallJumpDir);
            }
        }

        private void HandleJumpButtonDown()
        {
            if (Input.GetButtonDown("Jump"))
            {
                lastPressedJumpTime = data.jumpInputBufferTime;
            }
        }

        private void HandleJumpButtonUp()
        {
            if (Input.GetButtonUp("Jump"))
            {
                if (CanJumpCut())
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * data.jumpCutYVelocity);
                }
            }
        }

        private void HandleCoyoteTime()
        {
            if (isJumping)
                return;

            //Ground Check
            if (IsGrounded) //checks if set box overlaps with ground
            {
                lastOnGroundTime = data.coyoteTime; //if so sets the lastGrounded to coyoteTime
            }

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && isFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && !isFacingRight)) && !isWallJumping)
                lastOnWallRightTime = data.coyoteTime;

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && !isFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, groundLayer) && isFacingRight)) && !isWallJumping)
                lastOnWallLeftTime = data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);
        }

        private void HandleDeceleration()
        {
            if (movementInput.x == 0)
                return;

            float speedDifference = data.runMaxSpeed - Mathf.Abs(rb.linearVelocity.x);
            float deceleration = speedDifference * data.runDecceleration;

            rb.AddForce(Vector2.right * deceleration * movementInput.x);
        }

        private void HandleAcceleration()
        {
            if (movementInput.x != 0)
                return;

            float speedDifference = -rb.linearVelocity.x;
            float acceleration = speedDifference * data.runAcceleration;

            rb.AddForce(Vector2.right * acceleration);
        }

        private void HandleLookDirection()
        {
            if (movementInput.x < 0)
                isFacingRight = false;
            else if (movementInput.x > 0)
                isFacingRight = true;

            if (!isFacingRight)
                transform.localScale = new Vector3(-1, 1, 1); // change to -1 on X if we want to rotate the character.
            else if (isFacingRight)
                transform.localScale = new Vector3(1, 1, 1);
        }

        private void HandleCharacterVisualMovement()
        {
            bool hasLeftInput = movementInput.x < 0;
            bool hasRightInput = movementInput.x > 0;
            bool movingLeftButInputRight = rb.linearVelocity.x < 0.01 && movementInput.x > 0;
            bool movingRightButInputLeft = rb.linearVelocity.x > 0.01 && movementInput.x < 0;

            // Handle Rotation
            float angle = 0;
            angle += hasLeftInput ? 15 : 0;
            angle += hasRightInput ? 15 : 0;
            angle += movingLeftButInputRight ? -30 : 0;
            angle += movingRightButInputLeft ? -30 : 0;
            Vector3 rot = new Vector3(0, 0, angle);
            visual.transform.localRotation = Quaternion.Euler(rot);

            // Handle Y Movement
            //float moveToY = 0;
            //moveToY += movingLeftButInputRight || movingRightButInputLeft ? -0.75f : 0;
            //MoveVisualToY(moveToY);
        }

        public void SetGravityScale(float scale)
        {
            rb.gravityScale = scale;
        }

        private bool CanJump()
        {
            return lastOnGroundTime > 0 && !isJumping;
        }

        private bool CanWallJumpCut()
        {
            return isWallJumping && rb.linearVelocity.y > 0;
        }

        private bool CanJumpCut()
        {
            return isJumping && rb.linearVelocity.y > 0;
        }

        private bool CanWallJump()
        {
            return lastPressedJumpTime > 0 && lastOnWallTime > 0 && lastOnGroundTime <= 0 && (!isWallJumping ||
                 (lastOnWallRightTime > 0 && lastWallJumpDir == 1) || (lastOnWallLeftTime > 0 && lastWallJumpDir == -1));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheck.localScale);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(frontWallCheck.position, frontWallCheck.localScale);
            Gizmos.DrawWireCube(backWallCheck.position, backWallCheck.localScale);
        }
    }
}