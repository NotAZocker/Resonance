﻿using System;
using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    enum ConnectionType
    {
        None,
        Portal,
        Door
    }

    [SerializeField] GameObject connectionObject, noConnectionObject;

    [SerializeField] Portal portal;
    public Portal Portal => portal;
    [SerializeField] bool isWindow;
    public bool IsWindow => isWindow;

    RoomConnector otherConnector;
    bool isConnected;

    ConnectionType connectionType;
    public bool IsConnected => isConnected;

    void Start()
    {
        if (!isConnected)
        {
            SetConnectionType(ConnectionType.None);
        }
    }

    private void Update()
    {
        if (isConnected && connectionType == ConnectionType.Portal)
        {
            if(portal.LinkedPortal == null)
            {
                Debug.LogWarning(name + "has lost conncected portal!");
                SetConnectionType(ConnectionType.None);
            }
        }
    }

    void SetConnectionType(ConnectionType connectionType)
    {
        switch (connectionType)
        {
            case ConnectionType.None:
                connectionObject.SetActive(false);
                portal.gameObject.SetActive(false);
                noConnectionObject.SetActive(true);
                isConnected = false;
                break;
            case ConnectionType.Portal:
                connectionObject.SetActive(true);
                portal.gameObject.SetActive(true);
                noConnectionObject.SetActive(false);
                isConnected = true;
                GetComponentInParent<RoomController>().SetHasPortal(true);
                break;
            case ConnectionType.Door:
                connectionObject.SetActive(true);
                portal.gameObject.SetActive(false);
                noConnectionObject.SetActive(false);
                isConnected = true;
                break;
        }

        this.connectionType = connectionType;
    }

    public float GetYRotation()
    {
        return transform.eulerAngles.y;
    }

    internal bool TrySetOtherConnector(RoomConnector otherConnector, bool usePortal)
    {
        if (isConnected)
        {
            return false;
        }

        if (usePortal) SetConnectionType(ConnectionType.Portal);
        else SetConnectionType(ConnectionType.Door);

        otherConnector.TrySetOtherConnector(this, usePortal);

        this.otherConnector = otherConnector;
        if (portal.LinkedPortal == null && usePortal)
        {
            Portal.Link(portal, otherConnector.portal);
            //otherConnector.transform.Rotate(Vector3.up, 180);
        }

        return true;
    }

    internal void DeleteConnection()
    {
        if (!isConnected) return;

        SetConnectionType(ConnectionType.None);

        portal.Unlink();

        if (otherConnector != null)
        {
            otherConnector.DeleteConnection();
        }
    }
}
