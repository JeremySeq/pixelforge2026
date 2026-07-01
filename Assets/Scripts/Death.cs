using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    [Header("Spawn")]
    private Vector3 spawn;
    private Vector3 spawnRot;

    [Header("Chaser")]
    public ChaserMovement chaser;

    private PlayerMovement playerMovementController;

    private bool ended;

    [Header("BlackScreen")]
    public GameObject blackScreenUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawn = transform.position;
        spawnRot = transform.eulerAngles;
        playerMovementController = GetComponent<PlayerMovement>();
        ended = false;
        blackScreenUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -75f)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            spawn = other.transform.position;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Chaser"))
        {
            Die();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("End"))
        {
            ended = true;
        }
    }

    public void Die()
    {
        if (ended)
        {
            StartCoroutine(LoadNextScene());
        }
        else
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = spawn;
            transform.eulerAngles = spawnRot;
            playerMovementController.ResetVelocity();
            GetComponent<CharacterController>().enabled = true;
            chaser.respawn();
        }
    }

    private IEnumerator LoadNextScene()
    {
        blackScreenUI.SetActive(true);

        yield return new WaitForSeconds(2f);
        // Next scene
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }
}
