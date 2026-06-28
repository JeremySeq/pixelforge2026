using UnityEngine;
using UnityEngine.Rendering;

public class Checkpoint : MonoBehaviour
{
    [Header("Spawn")]
    public Vector3 spawn = new Vector3(-3.44f, 6.8f, -22.2f);

    private PlayerMovement playerMovementController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovementController = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -75f)
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = spawn;
            playerMovementController.ResetVelocity();
            GetComponent<CharacterController>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            spawn = other.transform.position;
        }
    }
}
