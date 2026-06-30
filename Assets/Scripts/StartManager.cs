using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
  public void PlayGame()
  {
      int index = SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(index + 1);
  }

  public void QuitGame()
  {
      Application.Quit();
  }

  public void SetVolume(float sliderValue) {
    AudioListener.volume = sliderValue;
  }
}
