using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    [Header("Classification")]
    public Themes theme;
    public Difficulties difficulty;
}

[CustomEditor(typeof(RoomSO))]
public class RoomSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomSO room = (RoomSO)target;

        // Basic fields
        room.roomName = EditorGUILayout.TextField("Room Name", room.roomName);
        room.width = EditorGUILayout.IntField("Width", Mathf.Max(1, room.width));
        room.height = EditorGUILayout.IntField("Height", Mathf.Max(1, room.height));
        room.timeLimit = EditorGUILayout.FloatField("Time Limit", room.timeLimit);
        room.theme = (Themes)EditorGUILayout.EnumPopup("Theme", room.theme);
        room.difficulty = (Difficulties)EditorGUILayout.EnumPopup("Difficulty", room.difficulty);

        // Ensure array matches grid size
        int total = room.width * room.height;
        if (room.cellTypes == null || room.cellTypes.Length != total)
        {
            int[] newArray = new int[total];
            if (room.cellTypes != null)
                System.Array.Copy(room.cellTypes, newArray, Mathf.Min(room.cellTypes.Length, total));
            room.cellTypes = newArray;
        }

        // Grid display
        EditorGUILayout.LabelField("Cell Types Grid:");
        for (int y = 0; y < room.height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < room.width; x++)
            {
                int index = y * room.width + x;

                // Determine color based on number
                Color bg = GetColorForType(room.cellTypes[index]);

                // Draw colored box first
                Rect rect = GUILayoutUtility.GetRect(30, 30);
                EditorGUI.DrawRect(rect, bg);

                // Draw the number on top
                int newValue = EditorGUI.IntField(rect, room.cellTypes[index], new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                });

                if (newValue != room.cellTypes[index])
                    room.cellTypes[index] = newValue;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(room);
    }

    // Map numbers to colors
    private Color GetColorForType(int type)
    {
        return type switch
        {
            0 => Color.blue,
            1 => Color.yellow,
            2 => Color.red,
            3 => Color.green,
            _ => Color.white
        };
    }

    // Helper for creating a 1x1 texture of a solid color
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}


