using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public GameObject difficultyButtons;
    public GameObject Startbutton;

    void Start()
    {
        Canvas.ForceUpdateCanvases();
    }

    public void OnStartButton()
    {
        difficultyButtons.SetActive(true);
        Startbutton.SetActive(false);
    }

    public void StartEasy() => GameManager.Instance.StartRun(Difficulties.Easy);
    public void StartMedium() => GameManager.Instance.StartRun(Difficulties.Meduim);
    public void StartHard() => GameManager.Instance.StartRun(Difficulties.Hard);
    public void StartCrazy() => GameManager.Instance.StartRun(Difficulties.Crazy);

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
