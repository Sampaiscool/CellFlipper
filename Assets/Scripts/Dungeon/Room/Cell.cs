using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Image cellImage;
    public Sprite offSprite;
    public Sprite onSprite;
    public Button button;

    private Room parentRoom;
    private int index;
    public bool isOn;
    public int type;

    public void Init(int idx, Room parent, int type)
    {
        index = idx;
        parentRoom = parent;
        this.type = type;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => parentRoom.OnCellClicked(index));

        SetType(type);
    }

    public void SetType(int t)
    {
        type = t;
        SetState(type == 1);
    }

    public void SetState(bool state)
    {
        isOn = state;
        if (cellImage != null)
            cellImage.sprite = isOn ? onSprite : offSprite;
    }

    public void Toggle() => SetState(!isOn);

    // Call this before destroying the cell
    public void Cleanup()
    {
        if (button != null)
            button.onClick.RemoveAllListeners();
        cellImage = null;
        parentRoom = null;
    }
}
