using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] RoomController startRoom;

    RoomSpawner roomSpawner;

    RoomController lastSpawnedRoom;
    //FirstPersonController player;

    private void Awake()
    {
        roomSpawner = GetComponent<RoomSpawner>();
        lastSpawnedRoom = startRoom;
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) lastSpawnedRoom = roomSpawner.SpawnNewRandomRoom(lastSpawnedRoom);
    }
}