using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes!

public class SceneReloader : MonoBehaviour
{
    public void ResetScene()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}