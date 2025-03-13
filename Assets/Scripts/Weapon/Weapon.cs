using UnityEngine;

public class Weapon : MonoBehaviour, IInteract
{
    [Header("Settings")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float lerpSpeed = 5f;

    [SerializeField] LayerMask weaponLayer;

    private bool isMoving = false;

    public event System.Action OnInteract;

    void Start()
    {
        playerCamera = FindAnyObjectByType<PortalMainCamera>().gameObject;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, offset, Time.deltaTime * lerpSpeed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotation), Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(transform.localPosition, offset) < 0.01f &&
                Quaternion.Angle(transform.localRotation, Quaternion.Euler(rotation)) < 0.1f)
            {
                isMoving = false;
                transform.localPosition = offset;
                transform.localRotation = Quaternion.Euler(rotation);
            }
        }
    }

    public void Interact()
    {
        transform.SetParent(playerCamera.transform);
        CoreManager.Instance.SpawnCores();
        isMoving = true;

        int weaponLayerIndex = Mathf.RoundToInt(Mathf.Log(weaponLayer.value, 2));

        foreach (Transform child in transform)
        {
            child.gameObject.layer = weaponLayerIndex;
        }
        this.gameObject.layer = weaponLayerIndex; // Also set the main object’s layer

        GetComponent<Collider>().enabled = false;

        OnInteract?.Invoke();
    }
}
