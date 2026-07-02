using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRealLifeScene : MonoBehaviour
{
    public void EndScene()
    {
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2f);
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }
}