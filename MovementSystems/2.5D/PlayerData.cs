using UnityEngine;

namespace AndrewDowsett.MovementSystems
{
    [CreateAssetMenu(fileName = "New Player Data", menuName = "2.5D Controller/Player Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
    public class PlayerData : ScriptableObject
    {

        [HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
        [HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).

        [Header("Gravity")]
        public float fallGravityMultiplier; //Multiplier to the player's gravityScale when falling.
        public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
        public float fastFallGravityMultiplier; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
                                                //Seen in games such as Celeste, lets the player fall extra fast if they wish.
        public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.

        [Space(20)]

        [Header("Run")]
        public float runMaxSpeed; //Target speed we want the player to reach.
        public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
        [HideInInspector] public float runAccelerationAmount; //The actual force (multiplied with speedDiff) applied to the player.
        public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
        [HideInInspector] public float runDeccelerationAmount; //Actual force (multiplied with speedDiff) applied to the player .

        [Space(20)]

        [Header("Jump")]
        public float jumpHeight; //Height of the player's jump
        public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
        [HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.
        public float timeBeforeLandingCheck; //Time before landing check is performed after jumping. This is to prevent the player from 'landing' as soon as they jump.
        public float jumpCutGravityMultiplier; //Multiplier to increase gravity if the player releases the jump button while still jumping

        [Space(20)]

        [Header("Both Jumps")]
        [Range(0f, 1)] public float jumpHangGravityMultiplier; //Reduces gravity while close to the apex (desired max height) of the jump
        public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
        [Range(0f, 1)] public float jumpCutYVelocity; //Multiplies the players Y velocity by this amount

        [Space(20)]

        [Header("Assists")]
        [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
        [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

        [Space(20)]

        [Header("Unused at this time")]
        public bool doConserveMomentum = true;
        [Range(0f, 1)] public float accelerationInAir; //Multipliers applied to acceleration rate when airborne.
        [Range(0f, 1)] public float deccelerationInAir;
        public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
        [Space(5)]
        [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
        [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
        public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction
        public float jumpHangAccelerationMultiplier;
        public float jumpHangMaxSpeedMultiplier;

        //Unity Callback, called when the inspector updates
        public void Init()
        {
            //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
            gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

            //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
            gravityScale = gravityStrength / Physics2D.gravity.y;

            //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
            runAccelerationAmount = ((1 / Time.deltaTime) * runAcceleration) / runMaxSpeed;
            runDeccelerationAmount = ((1 / Time.deltaTime) * runDecceleration) / runMaxSpeed;

            //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
            jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

            runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
            runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        }
    }
}