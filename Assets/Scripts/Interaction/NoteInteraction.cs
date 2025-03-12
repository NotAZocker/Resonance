using UnityEngine;

public class NoteInteraction : MonoBehaviour, IInteract
{
    [Header("Settings")]
    [SerializeField] private NotesTextSO noteSO;
    [SerializeField] private NotesUI notesUI;

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
