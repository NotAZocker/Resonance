using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] Transform roomParent;
    [SerializeField] RoomController startRoom;
    [SerializeField] RoomController[] roomPrefabs;

    [SerializeField] float roomSpawnDistanceToPlayer = 20;

    FirstPersonController player;

    List<RoomController> rooms = new List<RoomController>();

    RoomController lastSpawnedRoom;

    private void Awake()
    {
        lastSpawnedRoom = startRoom;

        rooms.Add(startRoom);
    }

    private void Start()
    {
        player = FindAnyObjectByType<FirstPersonController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            lastSpawnedRoom = SpawnNewRandomRoom(lastSpawnedRoom);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnNewRandomRoom(GetRoomOutOfPlayerVision());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ConnectRandomRooms();
        }
    }

    RoomController GetRoomOutOfPlayerVision()
    {
        RoomController room;
        int safetyCounter = 0;
        do
        {
            room = rooms[UnityEngine.Random.Range(0, rooms.Count)];
            safetyCounter++;
        }
        while (IsInPlayerVision(room) && safetyCounter < 50);

        if (safetyCounter == 50) print("Safety spawn room in vision");

        return room;
    }

    private bool IsInPlayerVision(RoomController room)
    {
        RoomConnector[] connectors = room.RoomConnectors;

        for (int i = 0; i < connectors.Length; i++)
        {
            if (InInFrontOfPlayer(connectors[i].transform.position) 
                && !IsViewObstructed(connectors[i].transform.position)
                || IsCloseToPlayer(connectors[i].transform.position))
            {
                return true;
            }
        }

        return false;

        bool IsCloseToPlayer(Vector3 position)
        {
            bool isCloseToPlayer = Vector3.Distance(player.transform.position, position) < roomSpawnDistanceToPlayer;

            print("IsCloseToPlayer: " + isCloseToPlayer);

            return isCloseToPlayer;
        }

        bool IsViewObstructed(Vector3 position)
        {
            Vector3 playerPos = player.transform.position;
            Vector3 playerForward = player.transform.forward;

            Vector3 playerToPosition = position - playerPos;

            RaycastHit hit;
            if (Physics.Raycast(playerPos, playerToPosition, out hit, playerToPosition.magnitude - 3))
            {
                if (hit.collider)
                {
                    if (hit.collider.gameObject == player.gameObject) print("aodiwoawfobfwo");
                    else
                    {
                        print("view obstructed, by not player");
                    }
                    return true;
                }
            }

            return false;
        }

        bool InInFrontOfPlayer(Vector3 position)
        {
            Vector3 playerPos = player.transform.position;
            Vector3 playerForward = player.transform.forward;

            Vector3 playerToPosition = position - playerPos;

            float angle = Vector3.Angle(playerForward, playerToPosition);

            print("Angle: " + angle);

            return angle < 90;
        }
    }

    public RoomController SpawnNewRandomRoom(RoomController attachRoom)
    {
        RoomController newRoom = roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Length)];

        return SpawnNewRoom(attachRoom, newRoom);
    }

    public RoomController SpawnNewRoom(RoomController attachRoom, RoomController roomPrefab)
    {
        RoomController newRoom = Instantiate(roomPrefab, roomParent);
        newRoom.name = "Room " + rooms.Count + " - " + newRoom.name;

        if (!attachRoom.TryAttachRoom(newRoom))
        {
            Destroy(newRoom.gameObject);
            Debug.Log("Can't attach room to " + attachRoom.name + ". Destroying room " + newRoom.name);
        }
        else
        {
            rooms.Add(newRoom);
        }

        return newRoom;
    }

    private void ConnectRandomRooms()
    {
        int rand1;
        int rand2;

        int safetyCounter = 0;
        do
        {
            rand1 = UnityEngine.Random.Range(0, rooms.Count);
            safetyCounter++;
        }
        while (IsInPlayerVision(rooms[rand1]) && safetyCounter < 50);

        safetyCounter = 0;
        do
        {
            rand2 = UnityEngine.Random.Range(0, rooms.Count);
            safetyCounter++;
        }
        while ((rand2 == rand1 || IsInPlayerVision(rooms[rand2])) && safetyCounter < 50);

        if(!rooms[rand1].TryAttachRoom(rooms[rand2], true, false))
        {
            Debug.LogError("Can't attach room " + rand2 + " to room " + rand1);
        }
    }
}
