using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] RoomConnector[] roomConnectors;
    RoomController[] connectedRooms;

    [SerializeField] GameObject roomItemsParent1, roomItemsParent2;

    private void Awake()
    {
        roomItemsParent1.SetActive(true);
        roomItemsParent2.SetActive(false);
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

        if(connectionCount > 1)
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
        List<RoomConnector> shuffledConnectors = new List<RoomConnector>(roomConnectors);

        List<RoomConnector> connectorList = roomConnectors.ToList();

        for (int i = 0; i < connectorList.Count; i++)
        {
            int rand = Random.Range(0, connectorList.Count);

            shuffledConnectors.Add(connectorList[rand]);
            shuffledConnectors.RemoveAt(rand);
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

    public bool TryAttachRoom(RoomController otherRoom)
    {
        RoomConnector myConnector = GetFreeDoorConnector();
        if (myConnector == null)
        {
            Debug.LogError("No free connectors found in room: " + name);
            return false;
        }

        RoomConnector otherConnector = otherRoom.GetFreeDoorConnector();
        if (otherConnector == null)
        {
            Debug.LogError("No free connectors found in room: " + otherRoom.name);
            return false;
        }

        otherRoom.transform.Rotate(Vector3.up, CalculateNewRoomRotation(myConnector, otherConnector));

        myConnector.TrySetOtherConnector(otherConnector);

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
