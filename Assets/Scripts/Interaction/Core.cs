using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Core : MonoBehaviour, IInteract
{
    [Header("Settings")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float maxGlowDistance = 5f;
    [SerializeField] private float minGlowIntensity = 0f;
    [SerializeField] private float maxGlowIntensity = 5f;
    [SerializeField] private float maxBloomDistance = 5f;
    [SerializeField] private float coreBloomThreshold = 1f;
    [SerializeField] private float standardBloomThreshold = 2f;
    [SerializeField] private float smoothTime = 1f;
    [SerializeField] private Color baseEmissionColor;
    [SerializeField] private Color baseColor;

    private Material material;

    private float currentGlowIntensity = 0f;
    private float currentBloomThreshold = 0f;

    private float glowVelocity = 0f;
    private float bloomVelocity = 0f;

    public void Interact()
    {
        Debug.Log("Picked up Core: " + name);
        Destroy(gameObject);
    }

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("There is no renderer on this object!");
        }
    }

    private void Start()
    {
        globalVolume.profile.TryGet(out Bloom bloom);
        bloom.threshold.value = standardBloomThreshold;
        material.color = baseColor;
    }

    private void Update()
    {
        globalVolume.profile.TryGet(out Bloom bloom);

        if (weapon == null || material == null) return;

        float distance = Vector3.Distance(transform.position, weapon.transform.position);

        float glowFactor = Mathf.Clamp01(1 - (distance / maxGlowDistance)) * maxGlowIntensity;
        float bloomFactor = Mathf.Clamp01(1 - (distance / maxBloomDistance)) * coreBloomThreshold;

        float targetGlowIntensity = Mathf.Clamp01(1 - (distance / maxGlowDistance)) * maxGlowIntensity;
        float targetBloomThreshold = Mathf.Clamp01(1 - (distance / maxBloomDistance)) * coreBloomThreshold;

        currentGlowIntensity = Mathf.SmoothDamp(currentGlowIntensity, targetGlowIntensity, ref glowVelocity, smoothTime);
        currentBloomThreshold = Mathf.SmoothDamp(currentBloomThreshold, targetBloomThreshold, ref bloomVelocity, smoothTime);

        Color emissionColor = baseEmissionColor * currentGlowIntensity;
        bloom.threshold.value = currentBloomThreshold;

        if (distance <= maxGlowDistance)
        {
            material.SetColor("_EmissionColor", emissionColor);
            material.EnableKeyword("_EMISSION");
        }
        else
        {
            material.SetColor("_EmissionColor", baseColor);
            material.DisableKeyword("_EMISSION");
        }
    }

}
