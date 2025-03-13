using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private GameObject notesDisplay;

    private FirstPersonController firstPersonController;

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
    }

    private void Start()
    {
        HideNotesDisplay();
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
}
