using UnityEngine;

public class TestSpawnPortal : MonoBehaviour
{
    [SerializeField] Portal portal, portalPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Portal newPortal = Instantiate(portalPrefab, transform.position + Vector3.forward * 5, Quaternion.identity);
            newPortal.transform.Rotate(Vector3.up, 90);

            Portal.Link(portal, newPortal);
        }
    }
}
