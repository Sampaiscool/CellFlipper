using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class RoomClearUIManager : MonoBehaviour
{
    public static RoomClearUIManager Instance;

    public TMP_Text messageText; // Assign in inspector
    public float displayTime = 2f;

    private Coroutine currentCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowRoomCleared(string roomName)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowMessageCoroutine(roomName));
    }

    private IEnumerator ShowMessageCoroutine(string roomName)
    {
        messageText.text = $"You have cleared {roomName}!";
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        messageText.gameObject.SetActive(false);
        currentCoroutine = null;
    }
}
