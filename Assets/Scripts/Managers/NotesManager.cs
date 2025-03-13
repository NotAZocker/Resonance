using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public static NotesManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<Sprite> noteSprites;
    private List<Sprite> usedSprites;

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
        if(noteSprites.Count == 0)
        {
            for (int i = 0; i < usedSprites.Count; i++)
            {
                noteSprites.Add(usedSprites[i]);
            }
            return null;
        }

        Sprite note = noteSprites[0];

        usedSprites.Add(note);
        noteSprites.Remove(note);

        return note;
    }
}
