using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePreloader : MonoBehaviour
{
    private AsyncOperation asyncLoad;

    void Start()
    {
        // Start loading the game scene in the background
        asyncLoad = SceneManager.LoadSceneAsync("Game");
        asyncLoad.allowSceneActivation = false; // Prevent automatic switching
    }

    public void StartGame()
    {
        // Activate the loaded scene when the player starts the game
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = true;
        }
    }
}
