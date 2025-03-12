using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] RoomController startRoom;

    RoomSpawner roomSpawner;
    FirstPersonController player;

    List<RoomController> rooms = new List<RoomController>();

    RoomController lastSpawnedRoom;

    private void Awake()
    {
        roomSpawner = GetComponent<RoomSpawner>();
        lastSpawnedRoom = startRoom;
    }

    private void Start()
    {
        player = FindAnyObjectByType<FirstPersonController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            lastSpawnedRoom = roomSpawner.SpawnNewRandomRoom(lastSpawnedRoom);
            rooms.Add(lastSpawnedRoom);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ConnectRandomRooms();
        }
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


    }
}