using UnityEngine;

public class DeveloperPanel : MonoBehaviour
{
    public bool developerModeEnabled = true; // Toggle in inspector
    public RoomSO testRoom;                  // Room to spawn quickly
    public float panelWidth = 250f;

    private int selectedThemeIndex = 0;
    private int selectedDifficultyIndex = 0;

    private void OnGUI()
    {
        if (!developerModeEnabled) return;

        GUI.Box(new Rect(10, 10, panelWidth, 350), "Developer Panel");

        // --- Test Room ---
        if (GUI.Button(new Rect(20, 40, panelWidth - 40, 30), "Spawn Test Room"))
        {
            if (testRoom != null)
                DungeonManager.Instance.RespawnRoom(testRoom);
        }

        // --- Next Room ---
        if (GUI.Button(new Rect(20, 80, panelWidth - 40, 30), "Next Room"))
        {
            if (DungeonManager.Instance.CurrentRoom != null)
                DungeonManager.Instance.OnRoomSolved(DungeonManager.Instance.CurrentRoom);
        }

        // --- Theme Dropdown ---
        GUI.Label(new Rect(20, 160, panelWidth - 40, 20), $"Selected Theme: {(Themes)selectedThemeIndex}");
        if (GUI.Button(new Rect(20, 180, panelWidth - 40, 30), "Change Theme"))
        {
            selectedThemeIndex++;
            if (selectedThemeIndex >= System.Enum.GetValues(typeof(Themes)).Length)
                selectedThemeIndex = 0; // loop around

            DungeonManager.Instance.GenerateBlock(GameManager.Instance.currentDifficulty, (Themes)selectedThemeIndex); // update DungeonManager
        }

        // --- Difficulty Dropdown ---
        GUI.Label(new Rect(20, 220, panelWidth - 40, 20), $"Selected Difficulty: {(Difficulties)selectedDifficultyIndex}");
        if (GUI.Button(new Rect(20, 240, panelWidth - 40, 30), "Change Difficulty"))
        {
            selectedDifficultyIndex++;
            if (selectedDifficultyIndex >= System.Enum.GetValues(typeof(Difficulties)).Length)
                selectedDifficultyIndex = 0; // loop around

            GameManager.Instance.currentDifficulty = (Difficulties)selectedDifficultyIndex; // update GameManager
        }

        // --- Spawn 5 Random Rooms ---
        if (GUI.Button(new Rect(20, 280, panelWidth - 40, 30), "Spawn 5 Rooms"))
        {
            Themes selectedTheme = (Themes)selectedThemeIndex;
            Difficulties selectedDiff = (Difficulties)selectedDifficultyIndex;

            GameManager.Instance.currentDifficulty = selectedDiff; // update GameManager
            DungeonManager.Instance.GenerateBlock(selectedDiff, selectedTheme);
            DungeonManager.Instance.SpawnNextRoom();
        }
    }
}
