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

    // Leaves cell properties
    public int leavesCooldown = 0; // 0 = ready
    public Image leafOverlay;
    public float leafFadeDuration = 0.5f; // seconds

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

        switch (type)
        {
            case 0: // Normal cell, unflipped
                SetState(false);
                break;
            case 1: // Starts flipped
                SetState(true);
                break;
            case 2: // Leaves cell, unflipped but shows overlay if cooldown active
                SetState(false);
                if (leafOverlay != null)
                    leafOverlay.gameObject.SetActive(leavesCooldown > 0);
                break;
        }
    }

    public void SetState(bool state)
    {
        isOn = state;
        if (cellImage != null)
            cellImage.sprite = isOn ? onSprite : offSprite;
    }

    public void Toggle()
    {
        int currentTurn = GameManager.Instance.CurrentTurn;

        // If locked due to leaves cooldown, block flipping
        if (type == 2 && leavesCooldown > 0)
        {
            Debug.Log("Leaves block this cell!");
            return;
        }

        // Flip normally
        SetState(!isOn);

        // If type 2, apply leaves effect regardless of on/off
        if (type == 2)
        {
            leavesCooldown = 2; // lock for next 2 turns
            if (leafOverlay != null)
                ShowLeaves();
        }
    }


    // Called each turn to decrement cooldown
    public void TickCooldown()
    {
        if (type == 2 && leavesCooldown > 0)
        {
            leavesCooldown--;
            if (leavesCooldown <= 0 && leafOverlay != null)
                HideLeaves();
        }
    }

    public void ShowLeaves()
    {
        if (leafOverlay == null) return;

        leafOverlay.gameObject.SetActive(true);
        StopCoroutine("FadeLeavesOut"); // stop any previous fade
        Color c = leafOverlay.color;
        c.a = 1f;
        leafOverlay.color = c;
    }
    public void HideLeaves()
    {
        if (leafOverlay == null) return;

        StopCoroutine("FadeLeavesOut");
        StartCoroutine(FadeLeavesOut());
    }

    // Call this before destroying the cell
    public void Cleanup()
    {
        if (button != null)
            button.onClick.RemoveAllListeners();
        cellImage = null;
        parentRoom = null;
    }

    private System.Collections.IEnumerator FadeLeavesOut()
    {
        float elapsed = 0f;
        Color c = leafOverlay.color;
        while (elapsed < leafFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / leafFadeDuration);
            leafOverlay.color = c;
            yield return null;
        }
        c.a = 0f;
        leafOverlay.color = c;
        leafOverlay.gameObject.SetActive(false);
    }
}
