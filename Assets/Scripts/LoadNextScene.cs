using UnityEngine;

public class LoadNextScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLetter()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level_Letter");
    }


}
