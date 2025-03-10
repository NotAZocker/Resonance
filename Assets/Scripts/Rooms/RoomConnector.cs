using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    [SerializeField] GameObject connectionObject, noConnectionObject;

    [SerializeField] Portal portal;
    public Portal Portal => portal;

    RoomConnector otherConnector;
    bool isConnected;

    public bool IsConnected => isConnected;

    public void Awake()
    {
        SetConnection(false);
    }

    public void SetConnection(bool connected)
    {
        connectionObject.SetActive(connected);
        noConnectionObject.SetActive(!connected);

        isConnected = connected;
    }

    public Vector3 GetDirection()
    {
        return transform.forward;
    }

    internal bool TrySetOtherConnector(RoomConnector otherConnector)
    {
        if(isConnected) return false;

        SetConnection(true);

        otherConnector.TrySetOtherConnector(this);

        this.otherConnector = otherConnector;
        if(portal.LinkedPortal != null) Portal.Link(portal, otherConnector.portal);

        return true;
    }
}
