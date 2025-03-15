using System;
using UnityEngine;

public class NoteInteraction : Interactable
{
    [Header("Settings")]

    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] Material paperMaterial;

    Sprite noteSprite;

    static int count;

    private void Start()
    {
        noteSprite = NotesManager.Instance.GetNextNoteAndRemoveFromList();
        Texture2D noteTex = noteSprite.texture;

        Material tempMat = new Material(paperMaterial);
        tempMat.name = "mat_" + count++;
        tempMat.SetTexture("_BaseMap", noteTex);
        meshRenderer.material = tempMat;
    }

    public override void Interact()
    {
        UIManager.Instance.ShowNote(noteSprite);

        base.Interact();
    }
}
