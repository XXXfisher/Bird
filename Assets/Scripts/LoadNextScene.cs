using UnityEngine;

public class LoadNextScene : MonoBehaviour
{

    public string sceneName = "Level_Letter";

    public void LoadLetter()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[LoadNextScene] sceneName is empty.");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }


}
