using UnityEngine;

public class ChaserMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 7f;
    private Vector3 spawn;

    void Start()
    {
        spawn = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 target = player.position;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.LookAt(player);
    }

    public void respawn()
    {
        transform.position = spawn;
    }
}
