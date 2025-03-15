using System;
using UnityEngine;
using VInspector;

public class PlayerInteraction : MonoBehaviour
{
    [Foldout("Interaction Area")]
    [SerializeField] private float interactRange = 2f;
    [EndFoldout]
    [SerializeField] private float sphereCastRadius = 0.5f;

    [SerializeField] LayerMask interactableLayer;

    [SerializeField] private GameObject interactableShow;
    [SerializeField] private GameObject interactInputTip;

    private PlayerInput playerInput;

    public event Action<Interactable> OnInteract;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        interactableShow.SetActive(false);
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hitInfo, interactRange, interactableLayer))
        {
            interactableShow.SetActive(true);
            print("Hit: " + hitInfo.collider.gameObject.name);

            if (hitInfo.collider != null && hitInfo.collider.gameObject.TryGetComponent(out Interactable interactObj))
            {

                if (playerInput.Actions.Interact.WasPressedThisFrame())
                {
                    interactObj.Interact();

                    OnInteract(interactObj);

                    interactInputTip.SetActive(false);
                }
            }
        }
        else
        {
            interactableShow.SetActive(false);
        }
    }
}
