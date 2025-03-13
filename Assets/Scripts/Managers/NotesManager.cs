using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public static NotesManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<Sprite> noteSprites;

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

    public Sprite GetRandomNoteAndRemoveFromList()
    {
        Sprite note = noteSprites[0];

        noteSprites.Remove(note);

        return note;
    }
}
