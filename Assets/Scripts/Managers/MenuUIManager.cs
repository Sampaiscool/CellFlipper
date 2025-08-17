using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public void OnStartButton()
    {
        GameManager.Instance.StartNewRun();
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
