using UnityEngine;
using VInspector;

public class PlayerInteraction : MonoBehaviour
{
    [Foldout("Interaction Area")]
    [SerializeField] private float interactRange = 2f;
    [EndFoldout]

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange))
        {
            if (hitInfo.collider != null && hitInfo.collider.gameObject.TryGetComponent(out IInteract interactObj))
            {
                if (playerInput.Actions.Interact.WasPressedThisFrame())
                {
                    interactObj.Interact();
                }
            }
        }
    }
}
