using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Dungeon Settings")]
    public int dungeonLength = 5; // how many rooms per run
    public Difficulties difficulty = Difficulties.Easy; // example, can hook to menu
    public int seed = 0; // optional, for reproducibility

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewRun()
    {
        if (seed == 0)
        {
            seed = Random.Range(1, int.MaxValue); // random seed if not set
        }
        Random.InitState(seed);

        SceneManager.LoadScene("GameScene");
    }
}
