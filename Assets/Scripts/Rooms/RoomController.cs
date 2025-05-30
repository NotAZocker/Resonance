using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public Vector3 RoomSize => checkSpaceFreeCollider.size;

    [SerializeField] Interactable specialObject;

    RoomConnector[] roomConnectors;
    public RoomConnector[] RoomConnectors => roomConnectors;
    [SerializeField]
    RoomConnector[] windows;
    public RoomConnector[] Windows => windows;

    [SerializeField] GameObject roomItemsParent1, roomItemsParent2;

    [SerializeField] float collisionSizeFactor = 0.4f;

    [SerializeField] int roomSpawnDistance = 20;

    [SerializeField] GameObject furnitureParent;
    [SerializeField] int furnitureCloseHeightDifference = 30;
    bool hasPortal;
    Transform player;

    bool hasCollided;
    public bool HasCollided => hasCollided;

    BoxCollider checkSpaceFreeCollider;

    public event Action<RoomController> OnSpecialObjectCollected;

    private void Awake()
    {
        if (roomItemsParent1 != null) roomItemsParent1.SetActive(true);
        if (roomItemsParent2 != null) roomItemsParent2.SetActive(false);

        checkSpaceFreeCollider = GetComponent<BoxCollider>();

        roomConnectors = transform.GetComponentsInChildren<RoomConnector>()
                .Where(connector => !connector.IsWindow)
                .ToArray();
        windows = transform.GetComponentsInChildren<RoomConnector>()
                .Where(connector => connector.IsWindow)
                .ToArray();

        if (specialObject != null)
        {
            specialObject.OnInteract += SpecialObjectCollected;
        }
    }

    private void OnEnable()
    {
        PortalTravalerTeleportPlayer playerTeleport = FindAnyObjectByType<PortalTravalerTeleportPlayer>();
        if(playerTeleport == null)
        {
            Debug.LogWarning("No player found", this);
            return;
        }
        player = playerTeleport.transform;
        playerTeleport.OnTeleport += CheckPlayerClose;
    }

    private void OnDisable()
    {
        if(player == null) return;
        player.GetComponent<PortalTravalerTeleportPlayer>().OnTeleport -= CheckPlayerClose;
    }

    void SpecialObjectCollected()
    {
        OnSpecialObjectCollected?.Invoke(this);
    }

    bool IsIntersectingWithExistingObjects()
    {
        Vector3 halfExtents = new Vector3(RoomSize.x * collisionSizeFactor, 7, RoomSize.z * collisionSizeFactor);

        Collider[] colliders = Physics.OverlapBox(transform.position, halfExtents, transform.rotation);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.gameObject.GetComponent<RoomController>())
            {
                return true;
            }
        }

        Vector3 myPos = transform.position;

        foreach (RoomController room in WorldManager.Instance.Rooms) // not sure why the other thing alone does not work. Sometimes spawns 2 rooms at the same position
        {
            Vector3 roomPos = room.transform.position;

            if(Mathf.Abs(myPos.x - roomPos.x) > (RoomSize.x + room.RoomSize.x) * collisionSizeFactor) continue;
            if(Mathf.Abs(myPos.y - roomPos.y) > 7) continue;
            if(Mathf.Abs(myPos.z - roomPos.z) > (RoomSize.z + room.RoomSize.z) * collisionSizeFactor) continue;

            if (Vector3.Distance(transform.position, room.transform.position) < RoomSize.x / 2)
            {
                Debug.LogWarning(name + " is very close to " + room.name + " but not intersecting");
                return true;
            }
            Debug.LogWarning(name + " is not very close, but still intersecting with " + room.name);
            return true;
        }

        return false;
    }



    public bool TryDespawnRoom()
    {
        int connectionCount = 0;

        for (int i = 0; i < roomConnectors.Length; i++)
        {
            if (roomConnectors[i].IsConnected)
            {
                connectionCount++;
            }
        }

        if (connectionCount > 1)
        {
            Debug.LogError("Room has multiple connections");
            return false;
        }

        // remove connections

        for (int i = 0; i < roomConnectors.Length; i++)
        {
            roomConnectors[i].DeleteConnection();
        }

        // remove room

        Destroy(gameObject, 0.1f);

        return true;
    }

    public void ChangeRoomItems()
    {
        roomItemsParent1.SetActive(!roomItemsParent1.activeSelf);
        roomItemsParent2.SetActive(!roomItemsParent2.activeSelf);
    }

    internal RoomConnector GetFreeDoorConnector()
    {
        for (int i = 0; i < 50; i++)
        {
            int rand = UnityEngine.Random.Range(0, roomConnectors.Length);

            if (!roomConnectors[rand].IsConnected)
            {
                return roomConnectors[rand];
            }
        }

        return null;
    }

    public RoomConnector GetFreeWindow()
    {
        if (windows.Length == 0) return null;

        for (int i = 0; i < 50; i++)
        {
            int rand = UnityEngine.Random.Range(0, windows.Length);

            if (!windows[rand].IsConnected)
            {
                return windows[rand];
            }
        }

        return null;
    }

    public bool TryAttachRoom(RoomController otherRoom, bool usePortal = false, bool moveOtherRoom = true)
    {
        RoomConnector myConnector = GetFreeDoorConnector();
        if (myConnector == null)
        {
            Debug.Log("No free connectors found in room: " + name);
            return false;
        }

        RoomConnector otherConnector = otherRoom.GetFreeDoorConnector();
        if (otherConnector == null)
        {
            Debug.Log("No free connectors found in room: " + otherRoom.name);
            return false;
        }

        if (moveOtherRoom)
        {
            otherRoom.transform.Rotate(Vector3.up, CalculateNewRoomRotation(myConnector, otherConnector));
            otherRoom.transform.position = myConnector.transform.position - otherConnector.transform.position;
            otherRoom.CheckPlayerClose();

            while (otherRoom.IsIntersectingWithExistingObjects())
            {
                usePortal = true;
                otherRoom.transform.position += Vector3.up * roomSpawnDistance;
            }
        }

        myConnector.TrySetOtherConnector(otherConnector, usePortal);

        return true;
    }

    private float CalculateNewRoomRotation(RoomConnector myConnector, RoomConnector otherConnector)
    {
        float myYRotation = myConnector.GetYRotation();
        float otherYRotation = otherConnector.GetYRotation();

        float angle = myYRotation - otherYRotation + 180;

        return angle;
    }

    internal int GetConnectionCount()
    {
        int connections = 0;

        foreach (RoomConnector connector in roomConnectors)
        {
            if (connector.IsConnected)
            {
                connections++;
            }
        }
        return connections;
    }

    internal bool TryConnectWindow(RoomController roomController)
    {
        RoomConnector window = GetFreeWindow();

        if (window == null)
        {
            return false;
        }
        else
        {
            return window.TrySetOtherConnector(roomController.GetFreeWindow(), true);
        }
    }

    public void DisconnectRoom()
    {
        foreach (RoomConnector connector in roomConnectors)
        {
            if (connector.IsConnected)
            {
                connector.DeleteConnection();
            }
        }

        foreach (RoomConnector window in windows)
        {
            if (window.IsConnected)
            {
                window.DeleteConnection();
            }
        }
    }

    internal void RemoveRoom()
    {
        DisconnectRoom();
        if(gameObject != null) Destroy(gameObject);
    }
    void SetFurnitureParentActive(bool value)
    {
        if(furnitureParent != null) furnitureParent.SetActive(value);
    }
    public void SetHasPortal(bool value)
    {
        hasPortal = value;
        if (value == true)
        {
            SetFurnitureParentActive(true);
        }
    }

    public void CheckPlayerClose()
    {
        if (hasPortal) return;

        if(player == null)
        {
            player = FindAnyObjectByType<PortalTravalerTeleportPlayer>().transform;
        }

        if (Mathf.Abs(transform.position.y - player.position.y) < furnitureCloseHeightDifference)
        {
            SetFurnitureParentActive(true);
        }
        else
        {
            SetFurnitureParentActive(false);
        }
    }
}
