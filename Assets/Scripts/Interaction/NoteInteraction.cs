using System;
using UnityEngine;

public class NoteInteraction : MonoBehaviour, IInteract
{
    [Header("Settings")]
    [SerializeField] private NotesTextSO noteSO;

    private NotesUI notesUI;

    public event Action OnInteract;

    private void Awake()
    {
        notesUI = GameObject.FindWithTag("NotesUI").GetComponent<NotesUI>();
    }

    private void Start()
    {
        NotesManager.Instance.unreadNotesSOList.Add(noteSO);
    }

    public void Interact()
    {
        noteSO.isRead = true;
        notesUI.SetNotesUITitleAndText(noteSO);
        NotesManager.Instance.RemoveNoteFromList(noteSO);
        UIManager.Instance.ShowNotesDisplay();
    }
}
