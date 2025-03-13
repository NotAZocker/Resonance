using UnityEngine;

public class TestInteraction : Interactable
{

    public override void Interact()
    {
        Debug.Log(Random.Range(0, 100));
    }
}
