using UnityEngine;

public class CoreManager : MonoBehaviour
{
    public static CoreManager Instance { get; private set; }

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

    public void IncreaseCoreCount()
    {
        currentCoreAmount++;
    }
}
