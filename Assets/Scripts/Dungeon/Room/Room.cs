using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Assigned in Inspector")]
    public GameObject cellPrefab;
    public GridLayoutGroup gridLayout;

    private List<Cell> cells = new List<Cell>();
    public RoomSO roomData;
    private int width;
    private int height;

    public void BuildRoom(RoomSO data)
    {
        roomData = data;
        width = data.width;
        height = data.height;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = width;

        // Cleanup old cells and listeners
        foreach (Transform child in gridLayout.transform)
        {
            var cell = child.GetComponent<Cell>();
            if (cell != null)
                cell.Cleanup();
            Destroy(child.gameObject);
        }
        cells.Clear();

        int total = width * height;

        if (roomData.cellTypes.Length < total)
        {
            int[] newArray = new int[total];
            for (int i = 0; i < total; i++)
                newArray[i] = i < roomData.cellTypes.Length ? roomData.cellTypes[i] : 0;
            roomData.cellTypes = newArray;
        }

        for (int i = 0; i < total; i++)
        {
            var obj = Instantiate(cellPrefab, gridLayout.transform);
            var cell = obj.GetComponent<Cell>();
            int type = roomData.cellTypes[i];
            cell.Init(i, this, type);
            cells.Add(cell);
        }
    }

    public void OnCellClicked(int index)
    {
        Flip(index);
        Flip(index - width);
        Flip(index + width);
        if (index % width != 0) Flip(index - 1);
        if (index % width != width - 1) Flip(index + 1);

        if (IsSolved())
            DungeonManager.Instance.OnRoomSolved(this);
    }

    void Flip(int index)
    {
        if (index < 0 || index >= cells.Count) return;
        cells[index].Toggle();
    }

    bool IsSolved()
    {
        foreach (var c in cells)
            if (!c.isOn) return false;
        return true;
    }

    // Call this before destroying the room
    public void Cleanup()
    {
        foreach (var c in cells)
            c.Cleanup();
        cells.Clear();
    }
}
