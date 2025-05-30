using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackScreenFader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] Image blackScreen;

    private void Start()
    {
        blackScreen.color = new Color(0, 0, 0, 0);
        blackScreen.gameObject.SetActive(false);
    }

    public void StartFadeToBlack()
    {
        blackScreen.gameObject.SetActive(true);
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            blackScreen.color = new Color(0, 0, 0, timer / fadeDuration);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 1);

        SceneManager.LoadScene("Menu");
    }
}
