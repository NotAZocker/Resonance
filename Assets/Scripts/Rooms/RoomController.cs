using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] Vector2 roomSize;
    public Vector2 RoomSize => roomSize;

    [SerializeField] RoomConnector[] roomConnectors;
    public RoomConnector[] RoomConnectors => roomConnectors;
    RoomController[] connectedRooms;

    [SerializeField] GameObject roomItemsParent1, roomItemsParent2;

    [SerializeField] float collisionSizeFactor = 0.4f;

    [SerializeField] int roomSpawnDistance = 20;

    bool hasCollided;
    public bool HasCollided => hasCollided;

    private void Awake()
    {
        Setup();
    }
    private void Setup()
    {
        if(roomItemsParent1 != null) roomItemsParent1.SetActive(true);
        if(roomItemsParent2 != null) roomItemsParent2.SetActive(false);

        BoxCollider checkSpaceFreeCollider = GetComponent<BoxCollider>();
        checkSpaceFreeCollider.size = new Vector3(roomSize.x, 1, roomSize.y);

    }

    bool IsIntersectingWithExistingObjects(Vector2 size)
    {
        Vector3 halfExtents = new Vector3(size.x * collisionSizeFactor, 7, size.y * collisionSizeFactor);

        Collider[] colliders = Physics.OverlapBox(transform.position, halfExtents, transform.rotation);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.gameObject.GetComponent<RoomController>())
            {
                print("room collision with " + collider.name);
                return true;
            }
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
        // Put roomConnections in a randomly ordered list
        List<RoomConnector> connectorList = new List<RoomConnector>();

        foreach (RoomConnector connector in roomConnectors) 
        {
            connectorList.Add(connector);
        }

        List<RoomConnector> shuffledConnectors = new List<RoomConnector>();

        for (int i = 0; i < connectorList.Count; i++)
        {
            int rand = Random.Range(0, connectorList.Count);

            shuffledConnectors.Add(connectorList[rand]);
            connectorList.RemoveAt(rand);
        }

        for (int i = 0; i < shuffledConnectors.Count; i++)
        {
            if (!shuffledConnectors[i].IsConnected)
            {
                return shuffledConnectors[i];
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

            while (otherRoom.IsIntersectingWithExistingObjects(otherRoom.RoomSize))
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
}
