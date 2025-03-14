using UnityEngine;
using UnityEngine.SceneManagement;

public class Hacks : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}