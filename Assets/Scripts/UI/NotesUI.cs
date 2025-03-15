using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotesUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Image noteImage;

    internal void SetNote(Sprite sprite)
    {
        noteImage.sprite = sprite;

        if(NotesManager.Instance != null)
        {
            NotesManager.Instance.SetNodeRead(sprite);
        }
    }
}
