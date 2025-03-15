using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public static NotesManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<Sprite> noteSprites;
    private List<Sprite> usedSprites = new List<Sprite>();
    private HashSet<Sprite> readNotes = new HashSet<Sprite>();

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

    private void Start()
    {
        usedSprites = GetAllReadSprites();

        foreach (Sprite note in usedSprites)
        {
            noteSprites.Remove(note);
        }
    }

    public Sprite GetNextNoteAndRemoveFromList()
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

        noteSprites.RemoveAt(0);
        usedSprites.Add(note);

        return note;
    }

    public Texture2D GetTextureFromSprite(Sprite sprite)
    {
        return sprite.texture;
    }

    public void SetNodeRead(Sprite noteSprite)
    {
        if(readNotes.Contains(noteSprite))
        {
            return;
        }

        readNotes.Add(noteSprite);
        PlayerPrefs.SetString("readNoteIndices", PlayerPrefs.GetString("readNoteIndices") + noteSprites.IndexOf(noteSprite) + ";");
    }

    List<Sprite> GetAllReadSprites()
    {
        List<Sprite> readSprites = new List<Sprite>();

        foreach (var noteIndex in PlayerPrefs.GetString("readNoteIndices").Split(';'))
        {
            if (noteIndex == "")
            {
                continue;
            }

            readSprites.Add(noteSprites[int.Parse(noteIndex)]);
        }

        return readSprites;
    }
}
