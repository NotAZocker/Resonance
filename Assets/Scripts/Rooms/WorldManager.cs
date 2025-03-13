using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] Transform roomParent;
    [SerializeField] RoomController startRoom;
    [SerializeField] RoomController[] roomPrefabs;
    [SerializeField] List<RoomController> specialRooms;

    [SerializeField] float roomSpawnDistanceToPlayer = 20;

    [SerializeField] int startRoomCount = 10;
    [SerializeField] float playerMoveDistanceToSpawn = 15;
    [SerializeField] float timeToSpawnSpecialRoomCopy = 30;

    [Header("World Change Probabilities")]
    [SerializeField] float roomSpawnProbability = 5;
    [SerializeField] float roomDestroyProbabilityPerRoom = 1 / 5;
    [SerializeField] float roomConnectProbability = 3;
    [SerializeField] float repositionRoomProbability = 1;

    public static WorldManager Instance;
    FirstPersonController player;

    List<RoomController> rooms = new List<RoomController>();
    public List<RoomController> Rooms => rooms;
    List<RoomController> currentSpecialRoomCopies = new List<RoomController>();

    RoomController lastSpawnedRoom;

    Vector3 lastSpawnPlayerPosition;
    float spawnNextSpecialRoomCopyTimer;

    private void Awake()
    {
        Instance = this;

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

        SpawnCurrentSpecialRoomCopy();
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, lastSpawnPlayerPosition) > playerMoveDistanceToSpawn)
        {
            ChangeSomethingInTheWorld();
            lastSpawnPlayerPosition = player.transform.position;
        }


        spawnNextSpecialRoomCopyTimer -= Time.deltaTime;
        if (spawnNextSpecialRoomCopyTimer <= 0)
        {
            SpawnCurrentSpecialRoomCopy();
        }


        if (Input.GetKey(KeyCode.LeftControl))
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
    }

    void SpawnCurrentSpecialRoomCopy()
    {
        if (specialRooms.Count == 0)
        {
            return;
        }

        RoomController specialRoom = null;
        int safetyCounter = 0;

        while (specialRoom == null && safetyCounter < 50)
        {
            specialRoom = SpawnNewRoom(GetRoomOutOfPlayerVision(), specialRooms[0]);
            safetyCounter++;
        }

        if (specialRoom == null)
        {
            Debug.LogWarning("Didn't find place to spawn special room.");
            return;
        }

        currentSpecialRoomCopies.Add(specialRoom);

        specialRoom.OnSpecialObjectCollected += SpecialRoomFound;

        spawnNextSpecialRoomCopyTimer = timeToSpawnSpecialRoomCopy;
    }

    public void SpecialRoomFound(RoomController foundRoom)
    {
        specialRooms.RemoveAt(0);

        foreach (RoomController room in currentSpecialRoomCopies)
        {
            room.OnSpecialObjectCollected -= SpecialRoomFound;

            if (room != foundRoom)
            {
                rooms.Remove(room);
                room.RemoveRoom();
            }
        }

        currentSpecialRoomCopies.Clear();

        SpawnCurrentSpecialRoomCopy();
    }

    private void ChangeSomethingInTheWorld()
    {
        int roomDestroyProbability = (int)(rooms.Count * roomDestroyProbabilityPerRoom);
        float totalProbability = roomSpawnProbability + roomConnectProbability + roomDestroyProbability;
        float rand = UnityEngine.Random.Range(0, totalProbability);

        if (rand < roomDestroyProbability)
        {
            DestroyRoom(GetRoomOutOfPlayerVision(false, true));
            return;
        }

        rand -= roomDestroyProbability;
        if (rand < roomSpawnProbability)
        {
            SpawnNewRandomRoom(GetRoomOutOfPlayerVision());
            return;
        }

        rand -= roomSpawnProbability;
        if (rand < repositionRoomProbability)
        {
            RepositionRoom(GetRoomOutOfPlayerVision(true, true));
            return;
        }

        // No need to check further; if it's not destroy or spawn, it's connect
        ConnectRandomRooms();
    }

    private void DestroyRoom(RoomController roomController)
    {
        if (roomController == null)
        {
            print("No room to destroy found.");
            return;
        }

        rooms.Remove(roomController);

        roomController.RemoveRoom();
    }

    RoomController GetRoomOutOfPlayerVision(bool returnRandomRoomIfNothingFound = true, bool isEndRoom = false, bool includeSpecialRooms = false)
    {
        RoomController room;
        int safetyCounter = 0;
        do
        {
            room = rooms[UnityEngine.Random.Range(0, rooms.Count)];
            safetyCounter++;
        }
        while ((IsInPlayerVision(room) || isEndRoom && room.GetConnectionCount() > 1 || !includeSpecialRooms && (currentSpecialRoomCopies.Contains(room))) && safetyCounter < 50);

        if (safetyCounter == 50)
        {
            if (!returnRandomRoomIfNothingFound)
            {
                return null;
            }
            else
            {
                print("Safety choose room in vision");
            }
        }

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
        RoomController newRoom = null;
        int safetyCounter = 0;
        do
        {
            newRoom = roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Length)];
            safetyCounter++;

        } while (newRoom.name == attachRoom.name && safetyCounter < 50);

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

        if (newRoom.Windows.Length > 0)
        {
            for (int i = 0; i < currentSpecialRoomCopies.Count; i++)
            {
                if (newRoom.TryConnectWindow(currentSpecialRoomCopies[i]))
                {
                    break;
                }
            }
        }

        return newRoom;
    }

    private void RepositionRoom(RoomController roomController)
    {
        roomController.DisconnectRoom();

        RoomController attachRoom = GetRoomOutOfPlayerVision();
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
