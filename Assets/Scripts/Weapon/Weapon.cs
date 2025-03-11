using UnityEngine;

public class Weapon : MonoBehaviour, IInteract
{
    [Header("Settings")]
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 offset;

    public void Interact()
    {
        transform.SetParent(player.transform);
        transform.localPosition = offset;
        //transform.position = player.transform.position + offset;
    }
}
