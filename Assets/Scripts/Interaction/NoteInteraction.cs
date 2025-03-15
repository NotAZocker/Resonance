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

        // Set transparency
        SetMaterialTransparent(tempMat);

        meshRenderer.material = tempMat;
    }

    public override void Interact()
    {
        UIManager.Instance.ShowNote(noteSprite);
        base.Interact();
    }

    void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1); // 1 = Transparent (URP)
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
