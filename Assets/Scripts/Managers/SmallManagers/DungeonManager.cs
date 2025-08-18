using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomGroup
{
    [Header("Group Settings")]
    public Difficulties difficulty;
    public Themes theme;

    [Header("Rooms")]
    public List<RoomSO> rooms;
}

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    public List<RoomGroup> roomGroups;
    public GameObject roomPrefab;
    public Transform dungeonParent;

    private List<RoomSO> chosenRooms;
    private List<Themes> cachedThemes;
    private int currentIndex = -1;
    private Room currentRoom;

    private int blockIndex = 0;
    private const int ROOMS_PER_BLOCK = 5;

    void Awake()
    {
        Instance = this;
        cachedThemes = System.Enum.GetValues(typeof(Themes))
                                  .Cast<Themes>()
                                  .Where(t => t != Themes.None)
                                  .ToList();
    }

    void Start()
    {
        GenerateBlock(GameManager.Instance.currentDifficulty, Themes.None);
        SpawnNextRoom();
    }

    void GenerateBlock(Difficulties diff, Themes theme)
    {
        var group = roomGroups.FirstOrDefault(g => g.difficulty == diff && g.theme == theme);
        if (group == null || group.rooms.Count == 0)
        {
            Debug.LogWarning($"No rooms found for {diff} / {theme}");
            chosenRooms = new List<RoomSO>();
            return;
        }

        var pool = new List<RoomSO>(group.rooms);
        chosenRooms = new List<RoomSO>();
        int length = Mathf.Min(ROOMS_PER_BLOCK, pool.Count);

        for (int i = 0; i < length; i++)
        {
            int randIndex = Random.Range(0, pool.Count);
            chosenRooms.Add(pool[randIndex]);
            pool.RemoveAt(randIndex);
        }

        currentIndex = -1;
    }

    void SpawnNextRoom()
    {
        // Cleanup old room completely
        if (currentRoom != null)
        {
            currentRoom.Cleanup();
            Destroy(currentRoom.gameObject);
            currentRoom = null;
        }

        currentIndex++;
        while (currentIndex >= chosenRooms.Count)
        {
            blockIndex++;
            Themes nextTheme = cachedThemes[Random.Range(0, cachedThemes.Count)];
            GenerateBlock(Difficulties.Easy, nextTheme);
            currentIndex++;
        }

        var roomData = chosenRooms[currentIndex];
        var obj = Instantiate(roomPrefab, dungeonParent);
        currentRoom = obj.GetComponent<Room>();
        currentRoom.BuildRoom(roomData);
    }

    public void OnRoomSolved(Room solvedRoom)
    {
        SpawnNextRoom(); // SpawnNextRoom now handles cleanup
    }
}