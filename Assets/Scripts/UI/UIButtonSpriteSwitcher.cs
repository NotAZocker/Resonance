using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIButtonSpriteSwitcher : MonoBehaviour
{
    [SerializeField] Sprite normalSprite; // Default sprite
    [SerializeField] Sprite selectedSprite; // Sprite when clicked
    [SerializeField] GameObject textPanel;

    [SerializeField] bool isDefaultSelected = false;

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

    private void Start()
    {
        if (isDefaultSelected)
        {
            // OnButtonClick();
        }
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
        textPanel.SetActive(true);
    }

    public void ResetToNormal()
    {
        if(textPanel == null)
        {
            print(name + " has no text panel");
            return;
        }

        // print("Resetting to normal: " + gameObject.name);
        buttonImage.sprite = normalSprite;
        text.color = Color.white;
        textPanel.SetActive(false);
    }
}
