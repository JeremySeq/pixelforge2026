using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputActionAsset inputAsset;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction lookAction;
    Rigidbody rb;
    public Transform cameraTransform;

    float moveSensitivity = .5f;
    float jumpSensitivity = 1.0f;
    float lookSensitivity = .3f;

    float xRotation = 0f;
    float upperLookLimit = 90f;
    float lowerLookLimit = -90f;

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
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        float jumpValue = jumpAction.ReadValue<float>();
        Vector3 move = transform.right * moveValue.x + transform.forward * moveValue.y;
        rb.AddForce(move * moveSensitivity + Vector3.up * jumpValue * jumpSensitivity, ForceMode.VelocityChange);
    }
}
