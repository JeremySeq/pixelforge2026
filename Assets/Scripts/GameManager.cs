using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Renderers")]
    public int rendererNoFogIndex = 0;
    public int rendererWhiteFogIndex = 1;
    public int rendererBlackFogIndex = 2;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        var cameraData = mainCam.GetComponent<UniversalAdditionalCameraData>();
        if (cameraData == null) return;

        Debug.Log("well we got here at least");
        switch (scene.name)
        {
            case "Demo":
                cameraData.SetRenderer(rendererWhiteFogIndex);
                break;
            case "school sequce":
                cameraData.SetRenderer(rendererNoFogIndex);
                break;
            case "Scene 2_1":
                cameraData.SetRenderer(rendererBlackFogIndex);
                Debug.Log("set black renderer!");
                break;
            default:
                cameraData.SetRenderer(rendererNoFogIndex);
                break;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
