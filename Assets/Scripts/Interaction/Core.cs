using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Core : Interactable
{
    [Header("Settings")]
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minGlowIntensity = 0f;
    [SerializeField] private float maxGlowIntensity = 5f;
    [SerializeField] private float coreBloomThreshold = 1f;
    [SerializeField] private float standardBloomThreshold = 2f;
    [SerializeField] private float smoothTime = .5f;
    [SerializeField] private Color baseEmissionColor;
    [SerializeField] private Color baseColor;

    [SerializeField] private AudioClip pickupSound;
    [SerializeField] LayerMask weaponLayer;

    private Material material;
    private Bloom bloom;
    private GameObject weapon;
    private Volume globalVolume;
    private Color emissionColor;

    private float currentGlowIntensity = 0f;
    private float currentBloomThreshold = 0f;
    private float glowVelocity = 2f;
    private float bloomVelocity = 2f;

    private bool isMoving = false;

    public bool IsMoving
    {
        get { return isMoving; }
        set { isMoving = value; }
    }

    public override void Interact()
    {
        isMoving = true;
        CoreManager.Instance.IncreaseCoreCount(this);

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

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        globalVolume = FindAnyObjectByType<Volume>();
        weapon = FindAnyObjectByType<Weapon>().gameObject;

        if (renderer != null)
        {
            material = renderer.materials[2];
        }
        else
        {
            Debug.LogError("There is no renderer on this object!");
        }
    }

    private void Start()
    {
        if (globalVolume.profile.TryGet(out bloom))
        {
            bloom.threshold.value = standardBloomThreshold;
        }
        material.color = baseColor;
    }

    private void Update()
    {
        if (weapon == null || material == null) return;
        CoreGlow();
    }

    private void CoreGlow()
    {
        float distance = Vector3.Distance(transform.position, weapon.transform.position);

        float glowFactor = Mathf.Clamp01(1 - (distance / maxDistance)) * maxGlowIntensity;
        float bloomFactor = Mathf.Clamp01(1 - (distance / maxDistance)) * coreBloomThreshold;

        float targetGlowIntensity = Mathf.Lerp(minGlowIntensity, maxGlowIntensity, glowFactor);
        float targetBloomThreshold = Mathf.Lerp(standardBloomThreshold, coreBloomThreshold, bloomFactor);

        currentGlowIntensity = Mathf.SmoothDamp(currentGlowIntensity, targetGlowIntensity, ref glowVelocity, smoothTime);
        currentBloomThreshold = Mathf.SmoothDamp(currentBloomThreshold, targetBloomThreshold, ref bloomVelocity, smoothTime);

        emissionColor = baseEmissionColor.linear * currentGlowIntensity;
        bloom.threshold.value = currentBloomThreshold;

        if (distance <= maxDistance)
        {
            material.SetColor("_EmissionColor", emissionColor);
            material.EnableKeyword("_EMISSION");
        }
        else
        {
            material.DisableKeyword("_EMISSION");
        }
    }
}
