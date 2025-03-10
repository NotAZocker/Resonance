using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    #region Enumerators
    enum JumpMode
    {
        //apply a force with ForceMode.VelocityChange
        Force,
        //set the vertical velocity
        Velocity
    }
	#endregion


	#region Settings
	//references
	[Header("References")]
    [Tooltip("The Reference to  the characters Camera.")]
    [SerializeField]
    private Transform characterCamera = default;

    //rotation
    [Header("Rotaion")]
    [Tooltip("The ability to rotate horizontal and vertical.")]
    [SerializeField]
    private bool enableCameraControls = true;
    [Tooltip("The Angle limit for looking up in degrees.")]
    [Range(0.0f, 90.0f)]
    [SerializeField]
    public float upperVerticalRotationLimit = 90.0f;
    [Tooltip("The Angle limit for looking down in degrees.")]
    [Range(0.0f, 90.0f)]
    [SerializeField]
    public float lowerVerticalRotationLimit = 90.0f;

    //general movement
    [Header("General Movement")]
    [Tooltip("The ability to move, jump, sprint, or crouch. Each ability can be turned of individually too.")]
    [SerializeField]
    private bool enableMovementControls = true;
    [Tooltip("Override the gravity field with the global gravity value on awake, if true.")]
    [SerializeField]
    private bool useGlobalGravity = false;
    [Tooltip("The gravity used for the character.")]
    [SerializeField]
    private Vector3 gravity = new Vector3(0.0f, -30.0f, 0.0f);
    [Tooltip("The distance the ground check is extended below the character.")]
    [SerializeField]
    private float groundCheckHeight = 0.05f;
    [Tooltip("What layers the character uses as ground")]
    [SerializeField]
    private LayerMask groundLayers = ~0;

    //horizontal movement
    [Header("Horizontal Movement")]
    [Tooltip("The ability to move horizontally.")]
    [SerializeField]
    private bool enableMove = true;
    [Tooltip("The maximum movement speed.")]
    [SerializeField]
    public float movementSpeed = 5.0f;
    [Tooltip("The maximum acceleration whilde grounded.")]
    [SerializeField]
    public float GroundAcceleration = 40.0f;
    [Tooltip("The maximum acceleration when movememnt input is zero.")]
    [SerializeField]
    public float GroundDeceleration = 40.0f;
    [Tooltip("The maximum acceleration while in the air.")]
    [SerializeField]
    public float AirAcceleration = 7.5f;
    [Tooltip("The maximum jerk when starting to move. Lowering this value will allow small and percise steps.")]
    [SerializeField]
    public float StartJerk = 250;

    //jump
    [Header("Jump")]
    [Tooltip("The ability to jump.")]
    [SerializeField]
    private bool enableJump = true;
    [Tooltip("Coose between different mode of physics for jumping.")]
    [SerializeField]
    private JumpMode jumpMode = JumpMode.Velocity;
    [Tooltip("The maximum height the caracter reaches after jumping.")]
    [SerializeField]
    public float jumpHeight = 1.5f;
    [Tooltip("The Time the character is able to jump after loosing ground.")]
    [SerializeField]
    public float coyoteTime = 0.1f;
    [Tooltip("The Time the jump input is buffered for jumping, if jump ability is gained within that time.")]
    [SerializeField]
    public float jumpBuffering = 0.1f;

    //sprint
    [Header("Sprint")]
    [Tooltip("The ability to sprint.")]
    [SerializeField]
    private bool enableSprint = true;
    [Tooltip("If false, the Character has al limited sprint bar.")]
    [SerializeField]
    public bool unlimitedSprint = true;
    [Tooltip("The multiplier for movement speed while sprinting.")]
    [Range(0.0f, 5.0f)]
    [SerializeField]
    public float sprintSpeedMultiplier = 2.0f;
    [Tooltip("The time it takes to empty the entire sprint bar.")]
    [SerializeField]
    public float sprintDuration = 5.0f;
    [Tooltip("The multiplier to fill instead of empty the sprint bar.")]
    [SerializeField]
    public float sprintCooldownRatio = 0.5f;
    [Tooltip("The maximum angle the character is able to sprint in degrees. 0� is forward, 90� is sideward, 180� is backward.")]
    [Range(0.0f, 180.0f)]
    [SerializeField]
    public float maxSprintAngle = 95.0f;
    [Tooltip("Revoke the sprinting status, when movement input is zero or max sprint angle is exceeded.")]
    [SerializeField]
    public bool revokeIsSprinting = false;
    
    //crouch
    [Header("Crouch")]
    [Tooltip("The ability to crouch.")]
    [SerializeField]
    private bool enableCrouch = true;
    [Tooltip("The Characters Height while crouching.")]
    [SerializeField]
    public float crouchHeight = 1.5f;
    [Tooltip("The multiplier for movement speed while crouching.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    public float crouchSpeedMultiplier = 0.5f;
	#endregion
    
    
	#region Globals
	//references
	private Rigidbody ownRigidbody;
    private CapsuleCollider ownCollider;

    //general movement
    public bool IsGrounded { get; private set; } = false;
    private float timeSinceGrounded = float.PositiveInfinity;

    //horizontal movement
    private bool CanMove { get => enableMovementControls && enableMove; }
    private Vector2 movementInput = default;
    private float movementTime = 0.0f; //time since velocity was zero the last time
    
    //jump
    private bool CanJump { get => enableMovementControls && enableJump; }
    private float jumpAmount;
    private float timeSinceJumpInput = float.PositiveInfinity; //may only be used for jump buffering
    private bool coyoteJumpAvailable = false;

    //sprint
    private bool CanSprint { get => enableMovementControls && enableSprint; }
    public bool IsSprinting { get; private set; } = false;
    private float sprintRemaining = 1.0f;
    
    //crouch
    private bool CanCrouch { get => enableMovementControls && enableCrouch; }
    private bool isCrouching = false;
    public bool IsCrouching { get => isCrouching; private set => ToggleCrouch(value); }
    private float originalHeight;
	#endregion


	#region Unity Messages
	private void Awake()
    {
        ownRigidbody = GetComponent<Rigidbody>();
        ownCollider = GetComponent<CapsuleCollider>();
        originalHeight = ownCollider.height;

        //gravity
        if (useGlobalGravity)
		{
            gravity = Physics.gravity;
		}

        //precalculate jumpAmount
        switch (jumpMode)
        {
            case JumpMode.Force:
            case JumpMode.Velocity:
                jumpAmount = Mathf.Sqrt(-2.0f * gravity.y * jumpHeight);
                break;
        }
    }


    void Update()
    {
        CheckSprint();
    }


    void FixedUpdate()
    {
        CheckGround();
        MovementUpdate();
        CheckJump();
        GravityUpdate();
    }
	#endregion


	#region Input
    /// <summary>
    /// Rotates the player and it's Camera.
    /// </summary>
    /// <param name="rotation">The horizontal and vertical rotation in degrees.</param>
	public void InputRotation(Vector2 rotation)
    {
        if (enableCameraControls)
        {
            ownRigidbody.rotation *= Quaternion.Euler(0, rotation.x, 0);

            float pitch = characterCamera.localEulerAngles.x;
            if (pitch > 180.0f)
			{
                pitch -= 360.0f;
			}
            pitch = Mathf.Clamp(pitch - rotation.y, -upperVerticalRotationLimit, lowerVerticalRotationLimit);
            characterCamera.localEulerAngles = new Vector3(pitch, 0, 0);
        }
    }


    /// <summary>
    /// Sets the desired movement direction.
    /// </summary>
    /// <param name="movement">the desired movement direction</param>
    public void InputMovement(Vector2 movement)
    {
        movementInput = movement;

        if (revokeIsSprinting && movement.sqrMagnitude < 0.0001)
        {
            IsSprinting = false;
        }
    }


    /// <summary>
    /// Lets the Character Jump if possible.
    /// </summary>
    public void InputJump()
    {
        if (!CanJump)
        {
            return;
        }

        if (IsCrouching)
        {
            IsCrouching = false;
        }
        else
        {
            timeSinceJumpInput = 0.0f;
        }
    }


    /// <summary>
    /// Lets the Character Sprint if possible.
    /// </summary>
    /// <param name="sprint">Pass true to enter sprint state. Pass false to leave sprint state.</param>
    public void InputSprint(bool sprint)
    {
        if (!CanSprint)
        {
            return;
        }

        if (sprint)
        {
            if (IsCrouching)
            {
                IsCrouching = false;
            }
            IsSprinting = true;
        }
        else
        {
            IsSprinting = false;
        }
    }


    /// <summary>
    /// Lets the Character Crouch if possible.
    /// </summary>
    /// <param name="crouch">Pass true to enter crouch state. Pass false to leave crouch state.</param>
    public void InputCrouch(bool crouch)
    {
        if (!CanCrouch)
        {
            return;
        }

        if (crouch)
        {
            IsSprinting = false;
        }

        IsCrouching = crouch;
    }
    #endregion


    #region Enable Inputs
    /// <summary>
    /// The ability to rotate horizontal and vertical.
    /// </summary>
    public bool EnableRotationControls { get => enableCameraControls; set => enableCameraControls = value; }


    /// <summary>
    /// The ability to move, jump, sprint, or crouch. Each ability can be turned of individually too.
    /// </summary>
    public bool EnableMovementControls
    {
        get => enableMovementControls;
        set
        {
            enableMovementControls = value;
            if (!EnableMovementControls)
            {
                DisableMovement();
                DisableSprint();
                DisableCrouch();
            }
        }
    }
    /// <summary>
    /// The ability to move horizontally.
    /// </summary>
	public bool EnableMove
    {
        get => enableMove;
        set
        {
            enableMove = value;
            if (!enableMove)
            {
                DisableMovement();
            }
        }
    }


    /// <summary>
    /// The ability to jump.
    /// </summary>
    public bool EnableJump
    {
        get => enableJump;
        set => enableJump = value;
    }


    /// <summary>
    /// The ability to sprint.
    /// </summary>
    public bool EnableSprint
    {
        get => enableSprint;
        set
        {
            enableSprint = value;
            if (!enableSprint)
            {
                DisableSprint();
            }
        }
    }


    /// <summary>
    /// The ability to crouch.
    /// </summary>
    public bool EnableCrouch
    {
        get => enableCrouch;
        set
        {
            enableCrouch = value;
            if (!enableCrouch)
            {
                DisableCrouch();
            }
        }
    }
    #endregion


    #region Movement Physics
    /// <summary>
    /// Update routine for horizontal movement physics.
    /// </summary>
    private void MovementUpdate()
    {
        if (!CanMove)
		{
            return;
		}

        Vector3 targetVelocity3D = transform.TransformDirection(new Vector3(movementInput.x, 0, movementInput.y));
        Vector2 targetVelocity = new Vector2(targetVelocity3D.x, targetVelocity3D.z);
        targetVelocity = Vector2.ClampMagnitude(targetVelocity, 1.0f);

        float speedMultiplier = 1.0f;
        if (IsSprinting)
        {
            //check sprint angle
            {
                Vector2 velocity = Vector3To2(transform.InverseTransformDirection(ownRigidbody.linearVelocity));
                float angle = Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg;
                if (Mathf.Abs(angle) <= maxSprintAngle)
                {
                    speedMultiplier *= sprintSpeedMultiplier;
                }
                else
                {
                    if (revokeIsSprinting)
                    {
                        IsSprinting = false;
                    }
                }
            }
        }
        if (IsCrouching)
        {
            speedMultiplier *= crouchSpeedMultiplier;
        }
        targetVelocity *= movementSpeed * speedMultiplier;

        Vector2 currentvelocity = Vector3To2(ownRigidbody.linearVelocity);
        Vector2 acceleration = targetVelocity - currentvelocity;

        float maxAcceleration;
        if (IsGrounded || timeSinceGrounded < coyoteTime)
        {
            if (targetVelocity.magnitude < 0.001f)
            {
                maxAcceleration = GroundDeceleration;
            }
            else
            {
                maxAcceleration = GroundAcceleration;
            }
        }
        else
        {
            maxAcceleration = AirAcceleration;
        }

        if (currentvelocity.sqrMagnitude < 0.001f && movementInput.sqrMagnitude < 0.001f)
        {
            movementTime = 0.0f;
        }
        movementTime += Time.deltaTime;

        //limit maxAcceleration to prevent overshooting targetVelocity
        maxAcceleration = Mathf.Min(maxAcceleration, acceleration.magnitude / Time.fixedDeltaTime);
        maxAcceleration = Mathf.Min(maxAcceleration, StartJerk * movementTime);
        acceleration.Normalize();
        acceleration *= maxAcceleration;

        ownRigidbody.AddForce(Vector2To3(acceleration), ForceMode.Acceleration);
    }


    /// <summary>
    /// Update routine for applying gravity.
    /// </summary>
    private void GravityUpdate()
	{
        ownRigidbody.AddForce(gravity, ForceMode.Acceleration);
	}


    /// <summary>
    /// Update routine for checking jumping conditions.
    /// </summary>
    private void CheckJump()
    {
        if (!CanJump)
        {
            return;
        }

        if (IsGrounded)
		{
            coyoteJumpAvailable = true;
        }

        if (timeSinceJumpInput <= jumpBuffering)
        {
            if (IsGrounded || (coyoteJumpAvailable && timeSinceGrounded < coyoteTime))
            {
                Jump();

                timeSinceJumpInput = float.PositiveInfinity;
                coyoteJumpAvailable = false;
            }
            else
            {
                timeSinceJumpInput += Time.deltaTime;
            }
        }
    }


    /// <summary>
    /// Apply a jump after checking conditions
    /// </summary>
    private void Jump()
	{
        switch (jumpMode)
        {
            case JumpMode.Force:
                ownRigidbody.AddForce(new Vector3(0.0f, jumpAmount, 0.0f), ForceMode.VelocityChange);
                break;

            case JumpMode.Velocity:
                Vector3 velocity = ownRigidbody.linearVelocity;
                velocity.y = jumpAmount;
                ownRigidbody.linearVelocity = velocity;
                break;
        }
    }


    /// <summary>
    /// Update routine to check if characters sprinting capabilities.
    /// </summary>
    private void CheckSprint()
    {
        if (!CanSprint)
        {
            return;
        }

        //check sprint bar
        if (!unlimitedSprint)
        {
            if (sprintRemaining <= 0.0f)
            {
                IsSprinting = false;
            }

            if (IsSprinting)
            {
                sprintRemaining = Mathf.Max(sprintRemaining
                    - (Time.deltaTime * (1 / sprintDuration)), 0.0f);
            }
            else
            {
                sprintRemaining = Mathf.Min(sprintRemaining
                    + (Time.deltaTime * (1 / (sprintDuration * sprintCooldownRatio))), 1.0f);
            }
        }
    }


    /// <summary>
    /// Changes the Characters Collider on changing the crouching state.
    /// </summary>
    /// <param name="crouch">The crouching state.</param>
    private void ToggleCrouch(bool crouch)
    {
        if (crouch == IsCrouching)
        {
            return;
        }

        float srinkHeight = originalHeight - crouchHeight;
        float newHeight;
        float cameraHeightOffset;

        if (crouch)
        {
            newHeight = crouchHeight;
            cameraHeightOffset = -srinkHeight;
        }
        else
        {
            newHeight = originalHeight;
            cameraHeightOffset = srinkHeight;
        }

        float newColliderCenterY = newHeight * 0.5f;

        ownCollider.height = newHeight;
        ownCollider.center = new Vector3(0.0f, newColliderCenterY, 0.0f);
        characterCamera.Translate(new Vector3(0.0f, cameraHeightOffset, 0.0f));

        isCrouching = crouch;
    }


    /// <summary>
    /// Update routine for the ground check
    /// </summary>
    private void CheckGround()
    {
        float colliderRadius = ownCollider.radius;
        Vector3 position = transform.position;
        position.y += colliderRadius - groundCheckHeight;

        if (Physics.CheckSphere(position, colliderRadius, groundLayers))
        {
            IsGrounded = true;
            timeSinceGrounded = 0.0f;
        }
        else
        {
            IsGrounded = false;
            timeSinceGrounded += Time.deltaTime;
        }
    }



#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (IsGrounded)
        {
            Gizmos.color = transparentGreen;
        }
        else
        {
            Gizmos.color = transparentRed;
        }

        float colliderRadius = GetComponent<CapsuleCollider>().radius;
        Vector3 position = transform.position;
        position.y += colliderRadius - groundCheckHeight;

        //when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(position, colliderRadius);
    }
#endif
    #endregion


    #region Disabling Controls
    /// <summary>
    /// Stops the horizontal movement.
    /// </summary>
    private void DisableMovement()
    {
        Vector3 velocity = ownRigidbody.linearVelocity;
        velocity.x = 0.0f;
        velocity.z = 0.0f;
        ownRigidbody.linearVelocity = velocity;
    }


    /// <summary>
    /// Disables the sprint state.
    /// </summary>
    private void DisableSprint()
    {
        IsSprinting = false;
    }


    /// <summary>
    /// Disables the crouch state.
    /// </summary>
    private void DisableCrouch()
    {
        IsCrouching = false;
    }
    #endregion


    #region Helper Functions
    /// <summary>
    /// Returns the x anz z component of a Vector3 as a Vector2.
    /// </summary>
    private Vector2 Vector3To2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }


    /// <summary>
    /// Returns a Vector 3 with x and z according to x and y of a Vector2.
    /// </summary>
    private Vector3 Vector2To3(Vector2 v)
    {
        return new Vector3(v.x, 0.0f, v.y);
    }
    #endregion
}
