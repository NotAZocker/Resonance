using System;
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

    public float GetYRotation()
    {
        print(name + "; LocalEulerAngles: " + transform.eulerAngles);

        return transform.eulerAngles.y;
    }

    internal bool TrySetOtherConnector(RoomConnector otherConnector)
    {
        if(isConnected) return false;

        SetConnection(true);

        otherConnector.TrySetOtherConnector(this);

        this.otherConnector = otherConnector;
        if(portal.LinkedPortal == null) Portal.Link(portal, otherConnector.portal);

        return true;
    }

    internal void DeleteConnection()
    {
        if(!isConnected) return;

        SetConnection(false);

        portal.Unlink();

        if(otherConnector != null)
        {
            DeleteConnection();
        }
    }
}
