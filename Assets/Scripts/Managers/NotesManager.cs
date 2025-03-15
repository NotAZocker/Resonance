using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public static NotesManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<Sprite> noteSprites;
    List<Sprite> unreadNotesSprites = new List<Sprite>();
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

        foreach (Sprite note in noteSprites)
        {
            unreadNotesSprites.Add(note);
        }
    }

    private void Start()
    {
        usedSprites = GetAllReadSprites();

        foreach (Sprite note in usedSprites)
        {
            unreadNotesSprites.Remove(note);
        }
    }

    public Sprite GetNextNoteAndRemoveFromList()
    {
        if(unreadNotesSprites.Count == 0)
        {
            for (int i = 0; i < usedSprites.Count; i++)
            {
                unreadNotesSprites.Add(usedSprites[i]);
            }
            return null;
        }

        Sprite note = unreadNotesSprites[0];

        unreadNotesSprites.RemoveAt(0);
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

        PlayerPrefs.Save();

        Debug.Log("Saved Notes: " + PlayerPrefs.GetString("readNoteIndices"));
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
