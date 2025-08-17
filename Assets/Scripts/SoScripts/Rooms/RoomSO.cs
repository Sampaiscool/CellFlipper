using UnityEngine;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Dungeon/RoomSO")]
public class RoomSO : ScriptableObject
{
    [Header("Room Dimensions")]
    public int width = 3;
    public int height = 3;

    [Header("Cell Types (length = width * height)")]
    public int[] cellTypes; // 0 = inactive, 1 = normal, 2 = trap, etc.

    [Header("Optional Metadata")]
    public string roomName;
    public float timeLimit = 60f;
    public int difficultyLevel; // 0 = easy, 10 = hard
}
