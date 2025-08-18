using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

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
    private int blockIndex = 0;
    private const int ROOMS_PER_BLOCK = 5;

    // Expose current room for developer use
    public Room CurrentRoom => currentRoom;
    private Room currentRoom;

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

    public void GenerateBlock(Difficulties diff, Themes theme)
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

    public void SpawnNextRoom()
    {
        // Cleanup old room
        if (currentRoom != null)
        {
            currentRoom.Cleanup();
            Destroy(currentRoom.gameObject);
            currentRoom = null;
        }

        currentIndex++;

        // Finished block? pick a new theme
        if (currentIndex >= chosenRooms.Count)
        {
            blockIndex++;
            Themes nextTheme = cachedThemes[Random.Range(0, cachedThemes.Count)];
            GenerateBlock(GameManager.Instance.currentDifficulty, nextTheme);
            currentIndex = 0;
        }

        if (chosenRooms.Count == 0)
        {
            Debug.LogWarning("No rooms available in this block!");
            return;
        }

        var roomData = chosenRooms[currentIndex];
        var obj = Instantiate(roomPrefab, dungeonParent);
        currentRoom = obj.GetComponent<Room>();
        currentRoom.BuildRoom(roomData);

        Debug.Log($"[DungeonManager] Spawned room: {roomData.roomName}");
    }

    public void OnRoomSolved(Room solvedRoom)
    {
        RoomClearUIManager.Instance.ShowRoomCleared(solvedRoom.roomData.roomName);
        SpawnNextRoom();
    }

    // Developer: spawn any room
    public void SpawnDeveloperRoom(RoomSO roomData)
    {
        if (currentRoom != null)
        {
            currentRoom.Cleanup();
            Destroy(currentRoom.gameObject);
        }

        var obj = Instantiate(roomPrefab, dungeonParent);
        currentRoom = obj.GetComponent<Room>();
        currentRoom.BuildRoom(roomData);

        Debug.Log($"[DungeonManager] Developer room spawned: {roomData.roomName}");
    }

    // Developer: skip current block and pick a new theme immediately
    public void SkipBlock()
    {
        Themes nextTheme = cachedThemes[Random.Range(0, cachedThemes.Count)];
        GenerateBlock(GameManager.Instance.currentDifficulty, nextTheme);
        currentIndex = 0;
        SpawnNextRoom();
        Debug.Log($"[DungeonManager] Skipped block, new theme: {nextTheme}");
    }
}

[CustomEditor(typeof(DungeonManager))]
public class DungeonManagerEditor : Editor
{
    private Dictionary<RoomGroup, bool> foldouts = new();

    public override void OnInspectorGUI()
    {
        DungeonManager manager = (DungeonManager)target;

        // Basic references
        manager.roomPrefab = (GameObject)EditorGUILayout.ObjectField("Room Prefab", manager.roomPrefab, typeof(GameObject), false);
        manager.dungeonParent = (Transform)EditorGUILayout.ObjectField("Dungeon Parent", manager.dungeonParent, typeof(Transform), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Room Groups", EditorStyles.boldLabel);

        // Make sure foldouts exist
        if (manager.roomGroups != null)
        {
            for (int i = 0; i < manager.roomGroups.Count; i++)
            {
                RoomGroup group = manager.roomGroups[i];
                if (!foldouts.ContainsKey(group))
                    foldouts[group] = false;

                foldouts[group] = EditorGUILayout.Foldout(foldouts[group], $"Group {i + 1}: {group.theme} / {group.difficulty}", true);

                if (foldouts[group])
                {
                    EditorGUI.indentLevel++;

                    // Theme & difficulty
                    group.theme = (Themes)EditorGUILayout.EnumPopup("Theme", group.theme);
                    group.difficulty = (Difficulties)EditorGUILayout.EnumPopup("Difficulty", group.difficulty);

                    // Rooms list as dropdowns
                    if (group.rooms == null)
                        group.rooms = new List<RoomSO>();

                    int removeIndex = -1;
                    for (int r = 0; r < group.rooms.Count; r++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        group.rooms[r] = (RoomSO)EditorGUILayout.ObjectField($"Room {r + 1}", group.rooms[r], typeof(RoomSO), false);

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                            removeIndex = r;
                        EditorGUILayout.EndHorizontal();
                    }
                    if (removeIndex >= 0)
                        group.rooms.RemoveAt(removeIndex);

                    // Add room button
                    if (GUILayout.Button("Add Room"))
                        group.rooms.Add(null);

                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.Space();

        // Button to add a new RoomGroup
        if (GUILayout.Button("Add Room Group"))
            manager.roomGroups.Add(new RoomGroup());

        if (GUI.changed)
            EditorUtility.SetDirty(manager);
    }
}