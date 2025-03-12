using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector.Libs;

public class WorldManager : MonoBehaviour
{
    [SerializeField] Transform roomParent;
    [SerializeField] RoomController startRoom;
    [SerializeField] RoomController[] roomPrefabs;
    [SerializeField] List<RoomController> specialRooms;

    [SerializeField] float roomSpawnDistanceToPlayer = 20;

    [SerializeField] int startRoomCount = 10;
    [SerializeField] float playerMoveDistanceToSpawn = 15;

    [Header("World Change Probabilities")]
    [SerializeField] float roomSpawnProbability = 5;
    [SerializeField] float roomDestroyProbability = 1;
    [SerializeField] float roomConnectProbability = 3;

    FirstPersonController player;

    List<RoomController> rooms = new List<RoomController>();
    List<RoomController> placedSpecialRooms = new List<RoomController>();

    RoomController lastSpawnedRoom;

    Vector3 lastSpawnPlayerPosition;

    private void Awake()
    {
        lastSpawnedRoom = startRoom;

        rooms.Add(startRoom);
    }

    private void Start()
    {
        player = FindAnyObjectByType<FirstPersonController>();

        lastSpawnPlayerPosition = player.transform.position;

        for (int i = 0; i < startRoomCount; i++)
        {
            SpawnNewRandomRoom(GetRoomOutOfPlayerVision());
        }
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, lastSpawnPlayerPosition) > playerMoveDistanceToSpawn)
        {
            ChangeSomethingInTheWorld();
            lastSpawnPlayerPosition = player.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            lastSpawnedRoom = SpawnNewRandomRoom(lastSpawnedRoom);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnNewRandomRoom(GetRoomOutOfPlayerVision());
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

    private void ChangeSomethingInTheWorld()
    {
        int placeSpecialRoomProbability = GetPlaceSpecialRoomProbability();
        float totalProbability = roomSpawnProbability + roomConnectProbability + roomDestroyProbability + placeSpecialRoomProbability;
        float rand = UnityEngine.Random.Range(0, totalProbability);

        print("Change sth: " + rand);

        if (rand < roomDestroyProbability)
        {
            DestroyRoom(GetRoomOutOfPlayerVision(true));
            return;
        }

        rand -= roomDestroyProbability;
        if (rand < roomSpawnProbability)
        {
            SpawnNewRandomRoom(GetRoomOutOfPlayerVision());
            return;
        }

        rand -= roomSpawnProbability;
        if (rand < placeSpecialRoomProbability)
        {
            RoomController attachRoom = GetRoomOutOfPlayerVision();
            if (attachRoom.TryAttachRoom(specialRooms[0]))
            {
                placedSpecialRooms.Add(specialRooms[0]);
                specialRooms.RemoveAt(0);
            }

            return;
        }

        // No need to check further; if it's not destroy or spawn, it's connect
        ConnectRandomRooms();
    }

    int GetPlaceSpecialRoomProbability()
    {
        if (specialRooms.Count == 0) return 0;

        if (rooms.Count < 20) return 0;
        else return rooms.Count / specialRooms.Count;
    }

    private void DestroyRoom(RoomController roomController)
    {
        rooms.Remove(roomController);

        if (placedSpecialRooms.Contains(roomController))
        {
            specialRooms.AddAt(roomController, 0);
            placedSpecialRooms.Remove(roomController);
        }

        roomController.RemoveRoom();
    }

    RoomController GetRoomOutOfPlayerVision(bool isEndRoom = false)
    {
        RoomController room;
        int safetyCounter = 0;
        do
        {
            room = rooms[UnityEngine.Random.Range(0, rooms.Count)];
            safetyCounter++;
        }
        while ((IsInPlayerVision(room) || isEndRoom && room.GetConnectionCount() > 1) && safetyCounter < 50);

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

        if (!rooms[rand1].TryAttachRoom(rooms[rand2], true, false))
        {
            Debug.Log("Can't attach room " + rand2 + " to room " + rand1);
        }
    }
}
