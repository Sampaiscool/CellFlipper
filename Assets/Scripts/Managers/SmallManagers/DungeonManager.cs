using UnityEngine;
using System.Collections.Generic;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    [Header("Dungeon Setup")]
    public List<RoomSO> roomPool; // assign in inspector
    public GameObject roomPrefab; // the one generic prefab

    public Transform dungeonParent;

    private List<RoomSO> chosenRooms;
    private int currentIndex = -1;
    private Room currentRoom;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateDungeon();
        SpawnNextRoom();
    }

    void GenerateDungeon()
    {
        chosenRooms = new List<RoomSO>();

        List<RoomSO> shuffled = new List<RoomSO>(roomPool);
        int length = Mathf.Min(GameManager.Instance.dungeonLength, shuffled.Count);

        for (int i = 0; i < length; i++)
        {
            int randIndex = Random.Range(0, shuffled.Count);
            chosenRooms.Add(shuffled[randIndex]);
            shuffled.RemoveAt(randIndex);
        }
    }

    void SpawnNextRoom()
    {
        currentIndex++;
        if (currentIndex >= chosenRooms.Count)
        {
            Debug.Log("Dungeon complete!");
            return;
        }

        var roomData = chosenRooms[currentIndex];

        // Spawn under Canvas
        var obj = Instantiate(roomPrefab, dungeonParent);

        // Reset local position/scale to fit nicely in the parent
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
        }

        currentRoom = obj.GetComponent<Room>();
        currentRoom.BuildRoom(roomData);
    }

    public void OnRoomSolved(Room solvedRoom)
    {
        Destroy(solvedRoom.gameObject);
        SpawnNextRoom();
    }
}
