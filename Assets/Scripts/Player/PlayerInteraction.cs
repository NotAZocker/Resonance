using UnityEngine;
using VInspector;

public class PlayerInteraction : MonoBehaviour
{
    [Foldout("Interaction Area")]
    [SerializeField] private float interactRange = 2f;
    [EndFoldout]

    [SerializeField] LayerMask interactableLayer;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, interactableLayer))
        {
            print("Hit: " + hitInfo.collider.gameObject.name);

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
