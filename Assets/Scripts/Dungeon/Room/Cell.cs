using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    // SandstormCell properties
    public Image sandstormOverlay;
    public float sandstormFadeDuration = 0.5f;

    public void Init(int idx, Room parent, int type)
    {
        index = idx;
        parentRoom = parent;
        this.type = type;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => parentRoom.OnCellClicked(index));

        SetType(type);

        if (type == 3 && sandstormOverlay != null)
        {
            sandstormOverlay.gameObject.SetActive(true);
            Color c = sandstormOverlay.color;
            c.a = 0.3f; // faint always
            sandstormOverlay.color = c;
        }
        else
        {
            sandstormOverlay.gameObject.SetActive(true);
            Color c = sandstormOverlay.color;
            c.a = 0.0f;
            sandstormOverlay.color = c;
        }
        if (type == 2 && leafOverlay != null)
        {
            leafOverlay.gameObject.SetActive(true);
            Color c = leafOverlay.color;
            c.a = 0.3f; // faint always
            leafOverlay.color = c;
        }
    }

    public void SetType(int t)
    {
        type = t;

        switch (type)
        {
            case 0: // Normal cell, starts OFF
                SetState(false);
                break;
            case 1: // Starts flipped ON
                SetState(true);
                break;
            case 2: // Leaves cell
                SetState(false);
                if (leafOverlay != null)
                    leafOverlay.gameObject.SetActive(leavesCooldown > 0);
                break;
            case 3: // Sandstorm cell
                SetState(false);
                break;
            case 4: // Broken cell: permanently ON and disabled
                SetState(true);
                if (button != null)
                    button.interactable = false;
                break;
        }
    }

    public void SetState(bool state, bool triggeredBySandstorm = false)
    {
        isOn = state;
        if (cellImage != null)
            cellImage.sprite = isOn ? onSprite : offSprite;

        if (triggeredBySandstorm)
            PlaySandstormEffect();
    }

    public void Toggle(bool triggeredBySandstorm = false)
    {
        if (type == 4) return; // broken
        if (type == 2 && leavesCooldown > 0) return;

        SetState(!isOn);

        if (type == 2)
        {
            leavesCooldown = 2;
            ShowLeaves();
        }

        // Sandstorm effect for triggered cells
        if (triggeredBySandstorm && sandstormOverlay != null)
        {
            float targetAlpha = (type == 3) ? 0.3f : 0f; // only type 3 remains faint
            StartCoroutine(SandstormRoutine(targetAlpha));
        }
    }

    // Let Room call this for visuals
    public void PlaySandstormEffectIfNeeded()
    {
        if (type == 3) PlaySandstormEffect();
    }

    public void PlaySandstormEffect()
    {
        if (sandstormOverlay != null)
        {
            float targetAlpha = type == 3 ? 0.3f : 0f; // fade back to faint for type 3, invisible otherwise
            StartCoroutine(SandstormRoutine(targetAlpha));
        }
    }

    private IEnumerator SandstormRoutine(float targetAlpha)
    {
        if (sandstormOverlay == null) yield break;

        sandstormOverlay.gameObject.SetActive(true);
        Color c = sandstormOverlay.color;
        c.a = 1f; // flash fully visible
        sandstormOverlay.color = c;

        float elapsed = 0f;
        while (elapsed < sandstormFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, targetAlpha, elapsed / sandstormFadeDuration);
            sandstormOverlay.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        sandstormOverlay.color = c;

        // if non-type-3, hide overlay after fade
        if (type != 3)
            sandstormOverlay.gameObject.SetActive(false);
    }

    // Called each turn to decrement cooldown
    public void TickCooldown()
    {
        if (type == 2 && leavesCooldown > 0)
        {
            leavesCooldown--;
            if (leavesCooldown <= 0 && leafOverlay != null)
            {
                // fade back to faint alpha only when cooldown ends
                StartCoroutine(FadeLeavesToAlpha(0.3f, leafFadeDuration));
            }
        }
    }

    public void ShowLeaves()
    {
        if (leafOverlay == null) return;

        leafOverlay.gameObject.SetActive(true);
        StopCoroutine(FadeLeavesToAlpha(0.3f, leafFadeDuration));

        // Only fully visible if active
        if (leavesCooldown > 0)
        {
            Color c = leafOverlay.color;
            c.a = 1f; // fully visible
            leafOverlay.color = c;
        }
    }

    public void HideLeaves()
    {
        if (leafOverlay == null) return;
        StopCoroutine("FadeLeavesOut");
        StartCoroutine(FadeLeavesToAlpha(0.3f, leafFadeDuration));
    }

    private IEnumerator FadeLeavesToAlpha(float targetAlpha, float duration)
    {
        float elapsed = 0f;
        Color c = leafOverlay.color;
        float startAlpha = c.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            leafOverlay.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        leafOverlay.color = c;
    }

    public void Cleanup()
    {
        if (button != null)
            button.onClick.RemoveAllListeners();
        cellImage = null;
        parentRoom = null;
    }
}
