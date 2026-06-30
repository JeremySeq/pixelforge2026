using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
  private bool isPaused;
  public GameObject pauseMenu;

  private InputActionAsset inputAsset;
  private InputAction escapeAction;

  void Start() {
    pauseMenu.SetActive(false);
    isPaused = false;
    inputAsset = InputSystem.actions;
    escapeAction = inputAsset.FindAction("Escape");
  }

  void Update() {
    if (escapeAction.WasPressedThisFrame()) {
      TogglePause();
    }
  }

  public void QuitGame()
  {
      Application.Quit();
  }

  public void SetVolume(float sliderValue) {
    AudioListener.volume = sliderValue;
  }

  public void Respawn() {
    Checkpoint player = FindAnyObjectByType<Checkpoint>();

    if (player != null)
    {
      player.Die();
    }
    
    TogglePause();
  }

  public void TogglePause() {
    isPaused = !isPaused;
    pauseMenu.SetActive(isPaused);
    PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();
    if (playerMovement != null) {
      playerMovement.enabled = !isPaused;
    }

    if (isPaused)
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    else
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
  }
}
