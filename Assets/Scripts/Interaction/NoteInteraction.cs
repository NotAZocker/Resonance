using System.Collections.Generic;
using UnityEngine;

public class NoteInteraction : MonoBehaviour, IInteract
{
    [Header("Settings")]
    [SerializeField] private List<NotesTextSO> noteTextSOList;

    public void Interact()
    {
        NotesTextSO randomNote = noteTextSOList[Random.Range(0, noteTextSOList.Count)];

        Debug.Log("Note Title: " + randomNote.name);
        Debug.Log("Note Text: " + randomNote.text);

        noteTextSOList.Remove(randomNote);
    }
}
