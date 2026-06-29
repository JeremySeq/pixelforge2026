using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    private int rendererWithFog = 0;
    private int rendererWithoutFog = 1;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        var cameraData = mainCam.GetComponent<UniversalAdditionalCameraData>();
        if (cameraData == null) return;

        if (scene.name == "MainGameplayScene")
        {
            cameraData.SetRenderer(0);
        }
        else if (scene.name == "RetroMinigameScene")
        {
            cameraData.SetRenderer(1);
        }

        switch (scene.name)
        {
            case "Demo":
                cameraData.SetRenderer(rendererWithFog);
                Debug.Log("ran Demo! renderer set to 0");
                break;
            case "school sequce":
                cameraData.SetRenderer(rendererWithoutFog);
                Debug.Log("ran school sequce! renderer set to 1");
                break;
            case "Scene 2_1":
                cameraData.SetRenderer(rendererWithFog);
                Debug.Log("ran Scene 2_1! renderer set to 0");
                break;
            default:
                cameraData.SetRenderer(rendererWithFog);
                Debug.Log("ran default! renderer set to 0");
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
