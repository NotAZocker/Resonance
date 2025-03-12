using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [SerializeField] Transform roomParent;

    [SerializeField] RoomController[] roomPrefabs;

    public RoomController SpawnNewRandomRoom(RoomController attachRoom)
    {
        RoomController newRoom = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

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
}
