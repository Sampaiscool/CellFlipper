using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject difficultyButtons;
    public GameObject startButton;
    public GameObject profileButton;
    public GameObject profilePanel;
    public GameObject shopPanel;

    [Header("Profile UI References")]
    public TMP_Text playerNameText;
    public TMP_Text titleText;
    public TMP_Text coinsText;
    public TMP_Text xpText;
    public Sprite profileImage;
    public Sprite backgroundImage;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
    }

    // === Start Buttons ===
    public void OnStartButton()
    {
        difficultyButtons.SetActive(true);
        profileButton.SetActive(false);
        startButton.SetActive(false);
    }

    public void StartEasy() => GameManager.Instance.StartRun(Difficulties.Easy);
    public void StartMedium() => GameManager.Instance.StartRun(Difficulties.Meduim);
    public void StartHard() => GameManager.Instance.StartRun(Difficulties.Hard);
    public void StartCrazy() => GameManager.Instance.StartRun(Difficulties.Crazy);

    public void OnQuitButton() => Application.Quit();

    // === Profile Panel ===
    public void OnProfileButton()
    {
        profileButton.SetActive(false);
        startButton.SetActive(false);
        profilePanel.SetActive(true);

        var stats = PlayerStats.Instance.statsSO;

        // Inside OnProfileButton()
        playerNameText.text = stats.playerName;
        titleText.text = stats.title.ToString();
        coinsText.text = "Coins: " + stats.coins;

        // Show Level instead of raw XP
        xpText.text = "Level: " + stats.GetLevel() + " (" + stats.experience + " XP)";

        // Load profile picture
        if (!string.IsNullOrEmpty(stats.profilePicture) && stats.profilePicture != "default")
        {
            Sprite profileSprite = Resources.Load<Sprite>(stats.profilePicture);
            if (profileSprite != null)
                profileImage = profileSprite;
        }

        // Load background image
        if (!string.IsNullOrEmpty(stats.background) && stats.background != "none")
        {
            Sprite bgSprite = Resources.Load<Sprite>(stats.background);
            if (bgSprite != null)
                backgroundImage = bgSprite;
        }
    }

    public void CloseProfile()
    {
        profilePanel.SetActive(false);
    }

    // === Shop Panel ===
    public void OnShopButton()
    {
        shopPanel.SetActive(true);
        // TODO: populate shop later
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }
}
