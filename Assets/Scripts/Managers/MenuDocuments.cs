﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuDocuments : MonoBehaviour
{
    [SerializeField]
    Button buttonPrefab;

    [SerializeField] Transform buttonParent;

    List<TMP_Text> buttonTexts = new List<TMP_Text>();

    [SerializeField] Sprite[] notes;
    List<int> readNoteIndices;

    NotesUI notesUI;

    private void Start()
    {
        notesUI = FindAnyObjectByType<NotesUI>();
        CloseDocument();

        for (int i = 0; i < notes.Length; i++) // spawn buttons for each note
        {
            int index = i; // Capture the loop variable
            Button button = Instantiate(buttonPrefab, buttonParent);
            button.onClick.AddListener(() => TryOpenDocument(index));
            buttonTexts.Add(button.GetComponentInChildren<TMP_Text>());
        }

        UpdateButtonTexts();
    }

    void UpdateButtonTexts()
    {
        readNoteIndices = GetAllReadIndices();

        for (int i = 0; i < buttonTexts.Count; i++)
        {
            if (readNoteIndices.Contains(i))
            {
                buttonTexts[i].text = "FILE " + (i+1).ToString("D3");
            }
            else
            {
                buttonTexts[i].text = "LOCKED";
            }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.O))
        {
            PlayerPrefs.SetString("readNoteIndices", "");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerPrefs.SetString("readNoteIndices", PlayerPrefs.GetString("readNoteIndices") + UnityEngine.Random.Range(0, notes.Length) + ";");
            print("Unlocked:" + PlayerPrefs.GetString("readNoteIndices"));
            UpdateButtonTexts();
        }
    }

    private void TryOpenDocument(int index)
    {
        if (!readNoteIndices.Contains(index)) return;

        notesUI.gameObject.SetActive(true);
        notesUI.SetNote(notes[index]);
    }

    public void CloseDocument()
    {
        notesUI.gameObject.SetActive(false);
    }

    List<int> GetAllReadIndices()
    {
        List<int> indices = new List<int>();

        foreach (var noteIndex in PlayerPrefs.GetString("readNoteIndices").Split(';'))
        {
            if (noteIndex == "")
            {
                continue;
            }

            indices.Add(int.Parse(noteIndex));
        }

        return indices;
    }
}