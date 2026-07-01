using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneCameraMovement : MonoBehaviour
{
    private InputAction lookAction;
    public float lookSensitivity = 0.05f;
    public float maxLookX = 10f;
    public float maxLookY = 8f;
    public float lookSmooth = 8f;
    public float returnSpeed = 1f;

    private Vector2 targetOffset;
    private Vector2 currentOffset;

    private Quaternion startingRotation;

    
    void Start()
    {
        lookAction = InputSystem.actions.FindAction("Look");
        startingRotation = transform.localRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouse = lookAction.ReadValue<Vector2>();

        targetOffset += mouse * lookSensitivity;

        // smoothly return to center when idle
        if (mouse.sqrMagnitude < 0.01f)
        {
            targetOffset = Vector2.Lerp(
                targetOffset,
                Vector2.zero,
                returnSpeed * Time.deltaTime
            );
        }

        if (targetOffset.magnitude > 1f)
        {
            Vector2 normalized = new Vector2(targetOffset.x / maxLookX, targetOffset.y / maxLookY);

            if (normalized.magnitude > 1f)
            {
                normalized.Normalize();
                targetOffset = new Vector2(normalized.x * maxLookX, normalized.y * maxLookY);
            }
        }

        // smooth camera motion
        currentOffset = Vector2.Lerp(currentOffset, targetOffset, lookSmooth * Time.deltaTime);
        transform.localRotation = startingRotation * Quaternion.Euler(-currentOffset.y, currentOffset.x, 0f);
    }
}
