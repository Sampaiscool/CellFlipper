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

    private bool[] initialOnStates;
    private int[] initialTypes;

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

        initialTypes = new int[cells.Count];
        initialOnStates = new bool[cells.Count];
        for (int i = 0; i < cells.Count; i++)
        {
            initialTypes[i] = cells[i].type;
            initialOnStates[i] = cells[i].isOn;
        }
    }

    public void OnCellClicked(int index)
    {
        Flip(index);                 // clicked cell
        Flip(index - width);         // above
        Flip(index + width);         // below
        if (index % width != 0) Flip(index - 1);          // left
        if (index % width != width - 1) Flip(index + 1);  // right

        GameManager.Instance.NextTurn();

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
    public void ResetRoom()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetType(initialTypes[i]);
            if (cells[i].isOn != initialOnStates[i])
            {
                cells[i].Toggle(); // flips back to original state
            }
            cells[i].leavesCooldown = 0;
        }
    }

    public void UpdateLeavesCells()
    {
        foreach (var cell in cells)
            cell.TickCooldown();
    }

    // Call this before destroying the room
    public void Cleanup()
    {
        foreach (var c in cells)
            c.Cleanup();
        cells.Clear();
    }
}
