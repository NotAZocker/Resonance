using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteract
{
    public event System.Action OnInteract;

    public void Interact()
    {
        Debug.Log(Random.Range(0, 100));
    }
}
