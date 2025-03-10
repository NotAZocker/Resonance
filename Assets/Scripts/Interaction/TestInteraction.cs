using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteract
{
    public void Interact()
    {
        Debug.Log(Random.Range(0, 100));
    }
}
