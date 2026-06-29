using UnityEngine;

public class ChaserAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip audioClip;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
