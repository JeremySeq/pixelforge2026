using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputAction moveAction;
    InputAction jumpAction;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        float jumpValue = jumpAction.ReadValue<float>();

        rb.AddForce(new Vector3(moveValue.x * 2, jumpValue * 5, moveValue.y * 2));
    }
}
