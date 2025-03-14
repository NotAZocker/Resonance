using System;
using UnityEngine;

public class NoteInteraction : Interactable
{
    [Header("Settings")]

    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer.sprite = NotesManager.Instance.GetRandomNoteAndRemoveFromList();
    }

    public override void Interact()
    {
        UIManager.Instance.ShowNote(spriteRenderer.sprite);

        base.Interact();
    }
}
