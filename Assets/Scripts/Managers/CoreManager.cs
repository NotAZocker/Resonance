using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    public static CoreManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<Core> coreList;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lerpSpeed = 2f;

    private Core core;
    private Weapon weapon;

    [HideInInspector] public GameObject coreSpotOne;
    [HideInInspector] public GameObject coreSpotTwo;
    [HideInInspector] public GameObject coreSpotThree;

    private int currentCoreAmount = 0;
    private int maxCoreAmount = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is more than one Instance of CoreManager in the Scene!");
        }
    }

    private void Start()
    {
        HideCores();
    }

    private void Update()
    {
        MoveCoresIntoWeapon(core);
        CheckCoreCount();
    }

    private void HideCores()
    {
        foreach (var core in coreList)
        {
            core.gameObject.SetActive(false);
        }
    }

    private void CheckCoreCount()
    {
        if (currentCoreAmount == maxCoreAmount)
        {
            FindAnyObjectByType<SceneChanger>().ChangeScene("Menu");
            Debug.Log("All Cores collected!");
        }
    }

    public void SpawnCores()
    {
        foreach (Core core in coreList)
        {
            core.gameObject.SetActive(true);
        }
    }

    public void IncreaseCoreCount(Core core)
    {
        weapon = GameObject.FindWithTag("Player").GetComponentInChildren<Weapon>();

        currentCoreAmount++;
        core.transform.SetParent(weapon.transform);
        this.core = core;
    }

    private void MoveCoresIntoWeapon(Core core)
    {
        if (core == null) return;

        Vector3 targetPosition = Vector3.zero;

        if (currentCoreAmount == 1)
        {
            targetPosition = coreSpotOne.transform.localPosition + offset;
        }
        else if (currentCoreAmount == 2)
        {
            targetPosition = coreSpotTwo.transform.localPosition + offset;
        }
        else if (currentCoreAmount == 3)
        {
            targetPosition = coreSpotThree.transform.localPosition + offset;
        }

        if (core.IsMoving)
        {
            core.transform.localPosition = Vector3.Lerp(core.transform.localPosition, targetPosition, lerpSpeed * Time.deltaTime);
            if (Vector3.Distance(core.transform.localPosition, targetPosition) < 0.01f)
            {
                core.IsMoving = false;
                core.transform.localPosition = targetPosition;
                core.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }
}
