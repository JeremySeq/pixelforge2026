using UnityEngine;

public class ChaserMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 7f;

    void Update()
    {
        if (player == null) return;

        Vector3 target = player.position;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.LookAt(player);
    }
}
