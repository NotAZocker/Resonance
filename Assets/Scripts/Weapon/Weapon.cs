using UnityEngine;

public class Weapon : MonoBehaviour, IInteract
{
    [Header("Settings")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float lerpSpeed = 5f;

    private bool isMoving = false;

    public event System.Action OnInteract;

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

        OnInteract?.Invoke();
    }
}
