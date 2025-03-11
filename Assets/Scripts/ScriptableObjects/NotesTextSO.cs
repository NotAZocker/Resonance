using UnityEngine;

[CreateAssetMenu(fileName = "NotesTextSO", menuName = "Scriptable Objects/NotesTextSO", order = 0)]
public class NotesTextSO : ScriptableObject
{
    public string noteName;
    [TextArea(15, 30)]
    public string text;
}
