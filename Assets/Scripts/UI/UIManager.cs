using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private GameObject notesDisplay;

    private FirstPersonController firstPersonController;
    private PlayerInput playerInput;
    private NotesUI notesUI;

    private InputAction interactAction;

    private bool isNoteOpen = false;
    private bool isInteractPerformed = false;

    private float timer;

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

        playerInput = FindAnyObjectByType<PlayerInput>();
        firstPersonController = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();
        notesUI = FindAnyObjectByType<NotesUI>();
        notesDisplay = notesUI.gameObject;
    }

    private void OnDisable()
    {
        interactAction.performed -= ToggleNotesDisplay;
    }

    private void Start()
    {
        interactAction = playerInput.Actions.Interact;
        interactAction.performed += ToggleNotesDisplay;
        HideNotesDisplay();

        Debug.Log("PlayerInput is: " + playerInput);
        Debug.Log("Interact action is: " + interactAction);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            HideNotesDisplay();
        }

        timer -= Time.deltaTime;
        isInteractPerformed = false;
    }

    private void ToggleNotesDisplay(InputAction.CallbackContext context)
    {
        if (isNoteOpen)
        {
            HideNotesDisplay();
        }
    }

    public void HideNotesDisplay()
    {
        if (timer > 0) return;
        Debug.Log("Hide Interact Performed: " + timer);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        firstPersonController.EnableMovementControls = true;
        notesDisplay.SetActive(false);
        if (isNoteOpen) timer = 0.1f;
        isNoteOpen = false;
        isInteractPerformed = true;
    }

    public void ShowNotesDisplay()
    {
        if (timer > 0) return;
        Debug.Log("Show Interact Performed: " + timer);


        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        firstPersonController.EnableMovementControls = false;
        notesDisplay.SetActive(true);
        isNoteOpen = true;
        isInteractPerformed = true;
        timer = 0.1f;
    }

    internal void ShowNote(Sprite sprite)
    {
        notesUI.SetNote(sprite);
        ShowNotesDisplay();
    }
}
