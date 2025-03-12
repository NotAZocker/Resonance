using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    public static CoreManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<Core> coreList;

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

    private void HideCores()
    {
        foreach (var core in coreList)
        {
            core.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        CheckCoreCount();
    }

    private void CheckCoreCount()
    {
        if (currentCoreAmount == maxCoreAmount)
        {
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

    public void IncreaseCoreCount()
    {
        currentCoreAmount++;
    }
}
