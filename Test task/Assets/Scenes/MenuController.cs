using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;



public class MenuController : MonoBehaviour
{   
    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ShowLeaderBoard()
    {
        SceneManager.LoadScene("LeaderBoard");
    }
}
