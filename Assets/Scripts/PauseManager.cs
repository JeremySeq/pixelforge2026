using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
  private bool isPaused;
  public GameObject pauseMenu;
  public Slider volumeSlider;

  private InputActionAsset inputAsset;
  private InputAction escapeAction;

  private float savedVolume;

  void Start() {
    pauseMenu.SetActive(false);
    volumeSlider.value = AudioListener.volume;
    savedVolume = AudioListener.volume;
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
    SceneManager.LoadScene(0);
    TogglePause();
  }

    public void SetVolume(float sliderValue) {
        savedVolume = sliderValue;
    }

  public void Respawn() {
    Death player = FindAnyObjectByType<Death>();

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
        AudioListener.volume = 0f;
    }
    else
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = savedVolume;
    }
  }
}
