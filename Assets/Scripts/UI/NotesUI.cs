using TMPro;
using UnityEngine;

public class NotesUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI noteTitle;
    [SerializeField] private TextMeshProUGUI noteText;

    public void SetNotesUITitleAndText(NotesTextSO note)
    {
        noteTitle.text = note.name;
        noteText.text = note.text;
    }
}
