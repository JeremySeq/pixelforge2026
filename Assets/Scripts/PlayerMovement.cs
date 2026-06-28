using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public Transform bottomOfPlayer;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float gravity = -12f;
    public float jumpHeight = 3.0f;

    [Header("Floatiness")]
    [Range(1f, 20f)] public float groundControl = 10f;
    [Range(1f, 20f)] public float airControl = 2.5f;

    [Header("Crouch")]
    public float standingHeight = 1f;
    public float crouchingHeight = .5f;
    public float crouchSpeed = 4f;
    public float heightLerpSpeed = 10f;

    [Header("Slide")]
    public float slideSpeed = 20f;
    public float slideDuration = .8f;
    public float slideFriction = .2f;
    [Tooltip("Speed necessary to toggle sliding with crouch button")]
    public float minSlideSpeed = 5f;
    public float slopeSlideSpeed = 12f;

    [Header("Abilities")]
    public bool canDoubleJump = true;
    public float doubleJumpHeight = 2.5f;
    public bool canWallJump = true;
    public float wallJumpStrength = 10.0f;
    public float wallRunGravityFactor = 0.5f;

    [Header("Camera Settings")]
    public GameObject camera;
    public float lookSensitivity = 0.3f;
    private Vector3 lookRotation;
    private Transform cameraTransform;
    private Camera cameraCamera;
    public float cameraTiltStrength = 2.0f;
    public float cameraTiltSpeed = 2.0f;
    public float wallRunTilt = 1.0f;
    private float currentTilt = 0.0f;

    private CharacterController controller;
    private InputActionAsset inputAsset;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    private InputAction crouchAction;

    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocity;

    private bool isGrounded;
    private int jumpCount = 0;

    private RaycastHit wallLeftHit;
    private RaycastHit wallRightHit;
    private bool wallLeft;
    private bool wallRight;
    private bool isCrouching;
    private bool isSliding;
    private float slideTimer;


    void Start()
    {
        controller = GetComponent<CharacterController>();

        inputAsset = InputSystem.actions;
        moveAction = inputAsset.FindAction("Move");
        jumpAction = inputAsset.FindAction("Jump");
        lookAction = inputAsset.FindAction("Look");
        crouchAction = inputAsset.FindAction("Crouch");

        cameraTransform = camera.transform;
        cameraCamera = camera.GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        // crouching
        isCrouching = crouchAction.IsPressed();
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        float currentHeight = this.gameObject.transform.localScale.y;
        float newHeight = Mathf.Lerp(
            currentHeight,
            targetHeight,
            Time.deltaTime * heightLerpSpeed
        );
        // scale using bottom of player as pivot
        ScaleAroundPivot(gameObject, bottomOfPlayer.position, new Vector3(gameObject.transform.localScale.x, newHeight, gameObject.transform.localScale.z));

        if (crouchAction.WasPressedThisFrame() && isGrounded && horizontalVelocity.magnitude > minSlideSpeed)
        {
            isSliding = true;
            slideTimer = slideDuration;
        }
        if (!isCrouching)
        {
            isSliding = false;
        }

        // check walls
        float dist = 2.0f;
        Debug.DrawRay(transform.position, -transform.right * dist, wallLeft ? Color.green : Color.red);
        Debug.DrawRay(transform.position, transform.right * dist, wallRight ? Color.green : Color.red);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out wallLeftHit, dist);
        if (wallLeft && wallLeftHit.collider.CompareTag("notWallrunnable"))
        {
            wallLeft = false;
        }
        wallRight = Physics.Raycast(transform.position, transform.right, out wallRightHit, dist);
        if (wallRight && wallRightHit.collider.CompareTag("notWallrunnable"))
        {
            wallRight = false;
        }

        wallLeft &= canWallJump;
        wallRight &= canWallJump;

        
        // ground movement
        isGrounded = controller.isGrounded;

        if ((isGrounded || wallRight || wallLeft) && verticalVelocity.y <= 0)
        {
            verticalVelocity.y = -2f;
            jumpCount = 0;
        }

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 targetDirection = transform.right * moveValue.x + transform.forward * moveValue.y;
                
        RaycastHit groundHit;
        float slopeThreshold = 45;
        bool onSlidable = Physics.Raycast(bottomOfPlayer.position, Vector3.down, out groundHit, dist) && Vector3.Angle(groundHit.normal, Vector3.up) > slopeThreshold && groundHit.collider.CompareTag("slidable");

        if (onSlidable) // forced slope sliding
        {
            Vector3 lookDir = cameraTransform.forward;
            // project look onto slope
            Vector3 slideDirection = Vector3.ProjectOnPlane(lookDir, groundHit.normal).normalized;
            // downhill direction
            Vector3 slopeDown = Vector3.ProjectOnPlane(Vector3.down, groundHit.normal).normalized;
            // angle between desired direction and downhill
            float angle = Vector3.SignedAngle(slopeDown, slideDirection, groundHit.normal);
            // can't slide more than 45 degrees away from downhill direction
            angle = Mathf.Clamp(angle, -45f, 45f);
            // rebuild direction from clamped angle
            slideDirection = Quaternion.AngleAxis(angle, groundHit.normal) * slopeDown;
            Debug.DrawRay(transform.position, slideDirection, Color.blue);
            horizontalVelocity = slideDirection * slopeSlideSpeed;
            controller.Move(horizontalVelocity * Time.deltaTime);
        }
        else if (isSliding) // ground sliding
        {
            slideTimer -= Time.deltaTime;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, slideFriction * Time.deltaTime);
            controller.Move(horizontalVelocity * Time.deltaTime);
            if (slideTimer <= 0f || horizontalVelocity.magnitude < minSlideSpeed)
            {
                isSliding = false;
            }
        }
        else // normal ground movement
        {
            float speed = isCrouching ? crouchSpeed : moveSpeed;
            Vector3 targetVelocity = targetDirection * speed;
            float currentLerpSpeed = isGrounded ? groundControl : airControl;
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, currentLerpSpeed * Time.deltaTime);
            controller.Move(horizontalVelocity * Time.deltaTime);
        }

        // jump off wall
        if (jumpAction.WasPressedThisFrame())
        {
            if (isGrounded || wallRight || wallLeft)
            {
                float wallRunBalancing = 1.0f;
                if (wallLeft || wallRight)
                    wallRunBalancing = 0.5f;

                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity * wallRunBalancing);
                jumpCount++;

                if (wallRight)
                    horizontalVelocity += -transform.right * wallJumpStrength;
                if (wallLeft)
                    horizontalVelocity += transform.right * wallJumpStrength;
            }
            else if (canDoubleJump && jumpCount < 2)
            {
                verticalVelocity.y = Mathf.Sqrt(doubleJumpHeight * -2f * gravity);
                jumpCount++;
            }
        }

        // wall movement
        if ((wallLeft || wallRight) && !isGrounded)
            verticalVelocity.y += gravity * Time.deltaTime * wallRunGravityFactor;
        else
            verticalVelocity.y += gravity * Time.deltaTime;

        controller.Move(verticalVelocity * Time.deltaTime);


        updateCameraRotation();
        
    }

    public void ResetVelocity()
    {
        horizontalVelocity = Vector3.zero;
        verticalVelocity = Vector3.zero;
    }

    public void ScaleAroundPivot(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 currentPosition = target.transform.position;
        Vector3 directionToTarget = currentPosition - pivot;
        float scaleFactor = newScale.x / target.transform.localScale.x; 
        Vector3 targetPositionPostScale = pivot + (directionToTarget * scaleFactor);
        target.transform.localScale = newScale;
        target.transform.position = targetPositionPostScale;
    }

    private void updateCameraRotation()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(horizontalVelocity);
        float targetTilt = -localVelocity.x * cameraTiltStrength;
        if (wallLeft)
            targetTilt -= wallRunTilt;
        if (wallRight)
            targetTilt += wallRunTilt;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * cameraTiltSpeed);

        Vector2 lookValue = lookAction.ReadValue<Vector2>();
        float mouseX = lookValue.x * lookSensitivity;
        float mouseY = lookValue.y * lookSensitivity;

        lookRotation.x -= mouseY;
        lookRotation.x = Mathf.Clamp(lookRotation.x, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(lookRotation.x, 0.0f, currentTilt);
        transform.Rotate(Vector3.up * mouseX);
    }
}
