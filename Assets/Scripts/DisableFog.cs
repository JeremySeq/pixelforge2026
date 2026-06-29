using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisableFog : MonoBehaviour
{
    public UniversalRendererData rendererData;
    public String featureName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature != null && feature.name == featureName)
            {
                feature.SetActive(false);
                rendererData.SetDirty();
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Called at end of scene
    void OnDisable()
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature != null && feature.name == featureName)
            {
                feature.SetActive(true);
                rendererData.SetDirty();
                break;
            }
        }
    }
}
