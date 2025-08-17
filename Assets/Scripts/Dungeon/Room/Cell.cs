using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [Header("Visuals")]
    public Image cellImage;           // The image renderer
    public Sprite offSprite;          // Sprite when OFF
    public Sprite onSprite;           // Sprite when ON

    [Header("Interaction")]
    public Button button;

    private Room parentRoom;
    private int index;
    public bool isOn;
    public int type; // 0 = inactive, 1 = normal, 2 = trap, etc.

    public void Init(int idx, Room parent)
    {
        index = idx;
        parentRoom = parent;
        button.onClick.AddListener(() => parentRoom.OnCellClicked(index));
        SetState(false);
    }

    public void SetType(int t)
    {
        type = t;

        // 0 = start OFF, 1 = start ON
        switch (t)
        {
            case 0:
                SetState(false);
                break;
            case 1:
                SetState(true);
                break;
            default:
                break;
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetState(bool state)
    {
        isOn = state;

        if (cellImage != null)
        {
            cellImage.sprite = isOn ? onSprite : offSprite;
        }
    }

    public void Toggle()
    {
        if (type == 0) return; // cannot toggle inactive cells
        SetState(!isOn);
    }
}
