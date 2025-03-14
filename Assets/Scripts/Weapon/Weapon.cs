using UnityEngine;

public class Weapon : Interactable
{
    [Header("Settings")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] LayerMask weaponLayer;

    private GameObject playerCamera;

    private bool isMoving = false;

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

    public override void Interact()
    {
        transform.SetParent(playerCamera.transform);
        CoreManager.Instance.SpawnCores();
        isMoving = true;

        CoreManager.Instance.coreSpotOne = transform.GetChild(0).gameObject;
        CoreManager.Instance.coreSpotTwo = transform.GetChild(1).gameObject;
        CoreManager.Instance.coreSpotThree = transform.GetChild(2).gameObject;

        int weaponLayerIndex = Mathf.RoundToInt(Mathf.Log(weaponLayer.value, 2));

        foreach (Transform child in transform)
        {
            child.gameObject.layer = weaponLayerIndex;
        }
        this.gameObject.layer = weaponLayerIndex; // Also set the main object’s layer

        GetComponent<Collider>().enabled = false;

        AudioManager.Instance.PlaySFX(pickupSound);

        base.Interact();
    }
}
