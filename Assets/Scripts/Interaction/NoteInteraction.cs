using System;
using UnityEngine;

public class NoteInteraction : MonoBehaviour, IInteract
{
    [Header("Settings")]

    [SerializeField] SpriteRenderer spriteRenderer;

    public event Action OnInteract;

    private void Start()
    {
        spriteRenderer.sprite = NotesManager.Instance.GetRandomNoteAndRemoveFromList();
    }

    public void Interact()
    {
        UIManager.Instance.ShowNote(spriteRenderer.sprite);
    }
}
