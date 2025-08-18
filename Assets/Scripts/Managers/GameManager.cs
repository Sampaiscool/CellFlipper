using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Run Settings")]
    public Difficulties currentDifficulty;
    public int dungeonLength = 5; // how many rooms per block
    public int blockIndex = 0;    // which 5-level block we’re on

    public List<RoomSO> currentPool = new List<RoomSO>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Canvas.ForceUpdateCanvases();
        }
        else Destroy(gameObject);
    }

    public void StartRun(Difficulties diff)
    {
        currentDifficulty = diff;
        blockIndex = 0;

        StartCoroutine(LoadSceneSafe("GameScene"));
    }

    private System.Collections.IEnumerator LoadSceneSafe(string sceneName)
    {
        // Yield one frame to let Unity settle temp allocations
        yield return null;
        SceneManager.LoadScene(sceneName);
    }

    public void NextBlock()
    {
        blockIndex++;
    }
}
