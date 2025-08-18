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

        int total = room.width * room.height;
        if (room.cellTypes == null || room.cellTypes.Length != total)
        {
            int[] newArray = new int[total];
            if (room.cellTypes != null)
                System.Array.Copy(room.cellTypes, newArray, Mathf.Min(room.cellTypes.Length, total));
            room.cellTypes = newArray;
        }

        EditorGUILayout.Space();

        // Reset button
        if (GUILayout.Button("Reset Grid"))
        {
            for (int i = 0; i < room.cellTypes.Length; i++)
                room.cellTypes[i] = 0; // all reset to type 0
        }

        EditorGUILayout.LabelField("Cell Types Grid:");

        for (int y = 0; y < room.height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < room.width; x++)
            {
                int index = y * room.width + x;

                // Draw colored box
                Rect rect = GUILayoutUtility.GetRect(30, 30);
                DrawCellVisual(rect, room.cellTypes[index]);

                // Draw number field on top
                int newValue = EditorGUI.IntField(rect, room.cellTypes[index], new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState() { textColor = Color.black }
                });

                if (newValue != room.cellTypes[index])
                    room.cellTypes[index] = newValue;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(room);
    }

    private void DrawCellVisual(Rect rect, int type)
    {
        switch (type)
        {
            case 0:
                EditorGUI.DrawRect(rect, Color.blue);
                break;
            case 1:
                EditorGUI.DrawRect(rect, Color.yellow);
                break;
            case 2:
                // Blue background
                EditorGUI.DrawRect(rect, Color.blue);

                // Draw a small green circle in the center
                Handles.BeginGUI();
                Color prev = Handles.color;
                Handles.color = Color.green;
                Vector2 center = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
                float radius = rect.width * 0.10f; // small circle
                Handles.DrawSolidDisc(center, Vector3.forward, radius);
                Handles.color = prev;
                Handles.EndGUI();
                break;
            case 3:
                EditorGUI.DrawRect(rect, Color.red);
                break;
            default:
                EditorGUI.DrawRect(rect, Color.white);
                break;
        }
    }
}


