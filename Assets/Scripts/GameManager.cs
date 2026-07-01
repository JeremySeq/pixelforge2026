using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Renderers")]
    public int rendererDreamIndex = 0;
    public int rendererRealLifeIndex = 1;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Demo":
                Dream();
                break;
            case "school sequce":
                Dream();
                break;
            case "Scene 1_1":
                Dream();
                break;
            case "Scene 1_1_1":
                Dream();
                break;
            case "Scene 1_2":
                RealLife();
                break;
            case "Scene 2_1":
                Dream();
                SetAmbientBlack();
                break;
            case "Scene 2_2":
                RealLife();
                break;
            case "Scene 3_1":
                Dream();
                SetAmbientBlack();
                break;
            default:
                Dream();
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

    void Dream()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;
        var cameraData = mainCam.GetComponent<UniversalAdditionalCameraData>();
        if (cameraData == null) return;
        cameraData.SetRenderer(rendererDreamIndex);
    }

    void RealLife()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;
        var cameraData = mainCam.GetComponent<UniversalAdditionalCameraData>();
        if (cameraData == null) return;
        cameraData.SetRenderer(rendererRealLifeIndex);
    }

    void SetAmbientBlack()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.ambientIntensity = 0.0f;
    }
}
