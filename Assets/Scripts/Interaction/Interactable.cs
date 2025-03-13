using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action OnInteract;

    public virtual void Interact()
    {
        OnInteract?.Invoke();
    }
}
