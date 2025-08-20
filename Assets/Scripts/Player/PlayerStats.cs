using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    public PlayerStatsSO statsSO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        statsSO.Load(); // Load data at startup
    }

    public void AddCoins(int amount)
    {
        statsSO.coins += amount;
        statsSO.Save();
    }

    public void AddXP(float amount)
    {
        statsSO.experience += amount;
        statsSO.Save();
    }

    public void SetProfile(string profile)
    {
        statsSO.profilePicture = profile;
        statsSO.Save();
    }

    public void SetBackground(string bg)
    {
        statsSO.background = bg;
        statsSO.Save();
    }

    public void SetPlayerName(string newName)
    {
        statsSO.playerName = newName;
        statsSO.Save();
    }

    public void SetTitle(Titles newTitle)
    {
        statsSO.title = newTitle;
        statsSO.Save();
    }
}
