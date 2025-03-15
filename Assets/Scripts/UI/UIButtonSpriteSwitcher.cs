using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIButtonSpriteSwitcher : MonoBehaviour
{
    public Sprite normalSprite; // Default sprite
    public Sprite selectedSprite; // Sprite when clicked

    private static List<UIButtonSpriteSwitcher> allButtons = new List<UIButtonSpriteSwitcher>();
    private Image buttonImage;

    TMP_Text text;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();

        buttonImage = GetComponent<Image>();
        if (!allButtons.Contains(this))
        {
            allButtons.Add(this);
        }

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        // Reset all other buttons
        foreach (UIButtonSpriteSwitcher button in allButtons)
        {
            button.ResetToNormal();
        }

        // Set the clicked button's sprite to selected
        buttonImage.sprite = selectedSprite;
        text.color = Color.black;
    }

    public void ResetToNormal()
    {
        // print("Resetting to normal: " + gameObject.name);
        buttonImage.sprite = normalSprite;
        text.color = Color.white;
    }
}
