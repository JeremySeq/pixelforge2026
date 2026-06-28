using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float gravity = -12f;
    public float jumpHeight = 3.0f;

    [Header("Floatiness")]
    [Range(1f, 20f)] public float groundControl = 10f;
    [Range(1f, 20f)] public float airControl = 2.5f;

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

    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocity;

    private bool isGrounded;
    private int jumpCount = 0;

    private RaycastHit wallLeftHit;
    private RaycastHit wallRightHit;
    private bool wallLeft;
    private bool wallRight;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        inputAsset = InputSystem.actions;
        moveAction = inputAsset.FindAction("Move");
        jumpAction = inputAsset.FindAction("Jump");
        lookAction = inputAsset.FindAction("Look");

        cameraTransform = camera.transform;
        cameraCamera = camera.GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float dist = 2.0f;
        Debug.DrawRay(transform.position, -transform.right * dist, wallLeft ? Color.green : Color.red);
        Debug.DrawRay(transform.position, transform.right * dist, wallRight ? Color.green : Color.red);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out wallLeftHit, dist);
        wallRight = Physics.Raycast(transform.position, transform.right, out wallRightHit, dist);
        wallLeft &= canWallJump;
        wallRight &= canWallJump;

        isGrounded = controller.isGrounded;

        if ((isGrounded || wallRight || wallLeft) && verticalVelocity.y <= 0)
        {
            verticalVelocity.y = -2f;
            jumpCount = 0;
        }

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 targetDirection = transform.right * moveValue.x + transform.forward * moveValue.y;
        Vector3 targetVelocity = targetDirection * moveSpeed;

        float currentLerpSpeed = isGrounded ? groundControl : airControl;

        horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, currentLerpSpeed * Time.deltaTime);
        controller.Move(horizontalVelocity * Time.deltaTime);

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

        if (wallLeft || wallRight)
            verticalVelocity.y += gravity * Time.deltaTime * wallRunGravityFactor;
        else
            verticalVelocity.y += gravity * Time.deltaTime;

        controller.Move(verticalVelocity * Time.deltaTime);

        // camera rotation

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
