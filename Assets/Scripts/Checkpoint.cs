using UnityEngine;
using UnityEngine.Rendering;

public class Checkpoint : MonoBehaviour
{
    Vector3 checkpoint = new Vector3(-3.44f, 6.8f, -22.2f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -75f)
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = checkpoint;
            GetComponent<CharacterController>().enabled = true;
        }
    }
}
