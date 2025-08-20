using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Default Values")]
    public string defaultPlayerName = "Player";
    public Titles defaultTitle = Titles.None;
    public float defaultExperience = 0f;
    public int defaultCoins = 0;
    public string defaultProfilePicture = "default";
    public string defaultBackground = "none";

    // Runtime values (not saved directly to SO)
    [HideInInspector] public string playerName;
    [HideInInspector] public Titles title;
    [HideInInspector] public float experience;
    [HideInInspector] public int coins;
    [HideInInspector] public string profilePicture;
    [HideInInspector] public string background;

    // PlayerPrefs keys
    private const string CoinsKey = "Player_Coins";
    private const string ProfilePicKey = "Player_Profile";
    private const string BackgroundKey = "Player_Background";
    private const string XPKey = "Player_XP";
    private const string TitleKey = "Player_Title";
    private const string NameKey = "Player_Name";

    // Load from PlayerPrefs or defaults
    public void Load()
    {
        coins = PlayerPrefs.GetInt(CoinsKey, defaultCoins);
        profilePicture = PlayerPrefs.GetString(ProfilePicKey, defaultProfilePicture);
        background = PlayerPrefs.GetString(BackgroundKey, defaultBackground);
        experience = PlayerPrefs.GetFloat(XPKey, defaultExperience);
        playerName = PlayerPrefs.GetString(NameKey, defaultPlayerName);
        title = (Titles)PlayerPrefs.GetInt(TitleKey, (int)defaultTitle);
    }

    // Save current runtime values
    public void Save()
    {
        PlayerPrefs.SetInt(CoinsKey, coins);
        PlayerPrefs.SetString(ProfilePicKey, profilePicture);
        PlayerPrefs.SetString(BackgroundKey, background);
        PlayerPrefs.SetFloat(XPKey, experience);
        PlayerPrefs.SetString(NameKey, playerName);
        PlayerPrefs.SetInt(TitleKey, (int)title);
        PlayerPrefs.Save();
    }

    // Reset to defaults
    public void ResetStats()
    {
        coins = defaultCoins;
        profilePicture = defaultProfilePicture;
        background = defaultBackground;
        experience = defaultExperience;
        playerName = defaultPlayerName;
        title = defaultTitle;
        Save();
    }

    // Level calculation
    public int GetLevel()
    {
        return Mathf.FloorToInt(experience / 100f) + 1; // 100 XP per level
    }
}
