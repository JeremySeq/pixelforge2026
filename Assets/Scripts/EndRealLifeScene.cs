using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRealLifeScene : MonoBehaviour
{
    public void EndScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }
}