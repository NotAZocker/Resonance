using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] Transform roomParent;
    [SerializeField] RoomController startRoom;
    [SerializeField] RoomController[] roomPrefabs;

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
            rooms.Add(lastSpawnedRoom);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rooms.Add(SpawnNewRandomRoom(rooms[UnityEngine.Random.Range(0, rooms.Count)]));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ConnectRandomRooms();
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

        if (!attachRoom.TryAttachRoom(newRoom))
        {
            Debug.LogError("Can't attach room to " + attachRoom.name);
        }

        return newRoom;
    }

    private void ConnectRandomRooms()
    {
        int rand1 = UnityEngine.Random.Range(0, rooms.Count);
        int rand2;
        do
        {
            rand2 = UnityEngine.Random.Range(0, rooms.Count);
        }
        while (rand2 == rand1);

        if(!rooms[rand1].TryAttachRoom(rooms[rand2], true, false))
        {
            Debug.LogError("Can't attach room " + rand2 + " to room " + rand1);
        }
    }
}
