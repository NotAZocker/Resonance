using UnityEngine;

public class TestRoomStuff : MonoBehaviour
{
    [SerializeField] RoomController roomPrefab;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            RoomController newRoom = Instantiate(roomPrefab, transform.position + Vector3.forward * 5, Quaternion.identity);
            newRoom.transform.Rotate(Vector3.up, 90);
        }
    }
}