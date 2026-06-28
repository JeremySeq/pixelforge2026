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
    public float wallJumpStrength = 50.0f;

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 0.3f;
    private float xRotation = 0f;

    private CharacterController controller;
    private InputActionAsset inputAsset;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;

    private Vector3 currentVelocity;
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
        Debug.Log(wallLeft);
        Debug.Log(wallRight);
        wallLeft &= canWallJump;
        wallRight &= canWallJump;

        Vector2 lookValue = lookAction.ReadValue<Vector2>();
        float mouseX = lookValue.x * lookSensitivity;
        float mouseY = lookValue.y * lookSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

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

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, currentLerpSpeed * Time.deltaTime);
        controller.Move(currentVelocity * Time.deltaTime);

        if (jumpAction.WasPressedThisFrame())
        {
            if (isGrounded || wallRight || wallLeft)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount++;
                if (wallRight)
                {
                    currentVelocity += -transform.right * wallJumpStrength;
                }
                if (wallLeft)
                {
                    currentVelocity += transform.right * wallJumpStrength;
                }
            }
            else if (canDoubleJump && jumpCount < 2)
            {
                verticalVelocity.y = Mathf.Sqrt(doubleJumpHeight * -2f * gravity);
                jumpCount++;
            }
        }

        if (wallLeft || wallRight)
        {
            verticalVelocity.y += gravity * Time.deltaTime / 3.0f;
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        controller.Move(verticalVelocity * Time.deltaTime);
    }
}
