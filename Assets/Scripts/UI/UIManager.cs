using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private GameObject notesDisplay;

    private FirstPersonController firstPersonController;

    NotesUI notesUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already a UIManager in the Scene.");
        }

        firstPersonController = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();
        notesUI = FindAnyObjectByType<NotesUI>();
        notesDisplay = notesUI.gameObject;
    }

    private void Start()
    {
        HideNotesDisplay();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideNotesDisplay();
        }
    }

    public void HideNotesDisplay()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        firstPersonController.EnableMovementControls = true;
        notesDisplay.SetActive(false);
    }

    public void ShowNotesDisplay()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        firstPersonController.EnableMovementControls = false;
        notesDisplay.SetActive(true);
    }

    internal void ShowNote(Sprite sprite)
    {
        notesUI.SetNote(sprite);

        ShowNotesDisplay();
    }
}
