using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public static NotesManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<NotesTextSO> noteTextSOList;

    public List<NotesTextSO> readNotesSOList;
    public List<NotesTextSO> unreadNotesSOList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already a NotesManager in the Scene.");
        }
    }

    public NotesTextSO GetRandomNoteAndRemoveFromList()
    {
        NotesTextSO randomNote = noteTextSOList[Random.Range(0, noteTextSOList.Count)];

        noteTextSOList.Remove(randomNote);

        return randomNote;
    }

    public void RemoveNoteFromList(NotesTextSO note)
    {
        if (note.isRead)
        {
            unreadNotesSOList.Remove(note);
            if (readNotesSOList.Contains(note) == note)
            {
                readNotesSOList.Remove(note);
            }
            readNotesSOList.Add(note);
            noteTextSOList.Remove(note);
        }
    }
}
