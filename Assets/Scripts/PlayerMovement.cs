using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Abilities")]
    public Boolean doubleJump = false;

    [Header("GameObject References")]
    public Transform cameraTransform;
    public CapsuleCollider groundCollider;

    InputActionAsset inputAsset;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction lookAction;
    Rigidbody rb;

    float moveSensitivity = 55f;
    float moveInAirSensitivity = 30f;
    float jumpSensitivity = 20f;
    float doubleJumpSensitivity = 25f;
    float lookSensitivity = .3f;

    float xRotation = 0f;
    float upperLookLimit = 90f;
    float lowerLookLimit = -90f;

    Boolean isGrounded = false;
    int jumpCount = 0;
    Boolean jumpPressedLastFrame = false;
    Boolean jumpPressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputAsset = InputSystem.actions;

        moveAction = inputAsset.FindAction("Move");
        jumpAction = inputAsset.FindAction("Jump");
        lookAction = inputAsset.FindAction("Look");

        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookValue = lookAction.ReadValue<Vector2>();

        float mouseX = lookValue.x * lookSensitivity;
        float mouseY = lookValue.y * lookSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, lowerLookLimit, upperLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void FixedUpdate()
    {
        float rayDist = .5f;
        Vector3 origin = transform.position + Vector3.up * rayDist;

        Collider[] hits = Physics.OverlapCapsule(
            groundCollider.bounds.center,
            groundCollider.bounds.center,
            groundCollider.radius
        );

        isGrounded = false;

        foreach (Collider hit in hits)
        {
            if (hit.transform.root != transform)
            {
                isGrounded = true;
                break;
            }
        }

        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        Vector3 move = transform.right * moveValue.x + transform.forward * moveValue.y;

        if (isGrounded)
        {
            rb.AddForce(move * moveSensitivity, ForceMode.Force);
        }
        else
        {
            rb.AddForce(move * moveInAirSensitivity, ForceMode.Force);
        }

        // jumping
        float jumpValue = jumpAction.ReadValue<float>();

        jumpPressed = jumpAction.ReadValue<float>() > 0.5f;

        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (jumpPressed && !jumpPressedLastFrame && jumpCount < (doubleJump ? 2 : 1))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * (jumpCount == 0 ? jumpSensitivity : doubleJumpSensitivity), ForceMode.Impulse);
            jumpCount++;
        }

        jumpPressedLastFrame = jumpPressed;


    }
}
