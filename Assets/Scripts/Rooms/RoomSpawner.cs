using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [SerializeField] RoomController startRoom;

    [SerializeField] RoomController[] roomPrefabs;

    [SerializeField] int roomSpawnDistance = 20;

    int roomsSpawned = 1;
    RoomController lastSpawnedRoom;

    private void Awake()
    {
        lastSpawnedRoom = startRoom;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) SpawnNewRandomRoom(lastSpawnedRoom);
    }

    public RoomController SpawnNewRandomRoom(RoomController attachRoom)
    {
        RoomController newRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)]);

        newRoom.transform.position = GetNextSpawnPosition();

        if (!attachRoom.TryAttachRoom(newRoom))
        {
            Debug.LogError("Failed to attach new room");
        }
        else
        {
            lastSpawnedRoom = newRoom;
            roomsSpawned++;
        }

        return newRoom;
    }

    private Vector3 GetNextSpawnPosition()
    {
        return roomsSpawned * roomSpawnDistance * Vector3.right;
    }
}

public class WorldManager : MonoBehaviour
{
    FirstPersonController player;
}