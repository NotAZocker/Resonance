using System;

public interface IInteract
{
    public event Action OnInteract;

    public void Interact();
}
