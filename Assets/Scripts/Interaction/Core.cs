using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Core : MonoBehaviour, IInteract
{
    [Header("Settings")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minGlowIntensity = 0f;
    [SerializeField] private float maxGlowIntensity = 5f;
    [SerializeField] private float coreBloomThreshold = 1f;
    [SerializeField] private float standardBloomThreshold = 2f;
    [SerializeField] private float smoothTime = 1f;
    [SerializeField] private Color baseEmissionColor;
    [SerializeField] private Color baseColor;

    private Material material;
    private Bloom bloom;

    private float currentGlowIntensity = 0f;
    private float currentBloomThreshold = 0f;

    private float glowVelocity = 0f;
    private float bloomVelocity = 0f;

    public void Interact()
    {
        CoreManager.Instance.IncreaseCoreCount();
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
        if (globalVolume.profile.TryGet(out bloom))
        {
            bloom.threshold.value = standardBloomThreshold;
        }
        material.color = baseColor;
    }

    private void Update()
    {
        if (weapon == null || material == null) return;

        float distance = Vector3.Distance(transform.position, weapon.transform.position);

        float glowFactor = Mathf.Clamp01(1 - (distance / maxDistance)) * maxGlowIntensity;
        float bloomFactor = Mathf.Clamp01(1 - (distance / maxDistance)) * coreBloomThreshold;

        float targetGlowIntensity = Mathf.Lerp(minGlowIntensity, maxGlowIntensity, glowFactor);
        float targetBloomThreshold = Mathf.Lerp(standardBloomThreshold, coreBloomThreshold, bloomFactor);

        currentGlowIntensity = Mathf.SmoothDamp(currentGlowIntensity, targetGlowIntensity, ref glowVelocity, smoothTime);
        currentBloomThreshold = Mathf.SmoothDamp(currentBloomThreshold, targetBloomThreshold, ref bloomVelocity, smoothTime);

        Color emissionColor = baseEmissionColor * currentGlowIntensity;
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
