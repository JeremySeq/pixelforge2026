using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.5f;
    public float minDelay = 0.05f;
    public float maxDelay = 0.2f;

    private Light light;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = transform.GetComponent<Light>();
        StartCoroutine(FlickerLoop());
    }

    IEnumerator FlickerLoop()
    {
        while (true)
        {
            light.intensity = Random.Range(minIntensity, maxIntensity);
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }
}
