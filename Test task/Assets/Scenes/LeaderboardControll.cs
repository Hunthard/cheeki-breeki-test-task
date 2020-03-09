using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerScore
{
    public int id;
    public string name;
    public int score;
    public float playedTime;

    public PlayerScore(int id, string name, int score, float playedTime)
    {
        this.id = id;
        this.name = name;
        this.score = score;
        this.playedTime = playedTime;
    }

    public PlayerScore Deserialize(string jsonString)
    {
        return JsonUtility.FromJson<PlayerScore>(jsonString);
    }
}

[System.Serializable]
public class LeaderBoard
{
    public List<PlayerScore> playerScores;
}

public class LeaderboardControll : MonoBehaviour
{
    private const string URL = "https://localhost:5001/api/leaderboard";
    public Text responseText;
    public int place;

    void Start()
    {
        StartCoroutine(GetLeaderboard());
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public IEnumerator GetLeaderboard()
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);
        request.certificateHandler = null;

        yield return request.SendWebRequest();

        if(request.isNetworkError) {
            Debug.Log(request.error);
            Debug.LogErrorFormat("error request [{0}, {1}]", URL, request.error);
            responseText.text = "Some problems here";
        }
        else {
            Debug.Log("request = " + request.downloadHandler.text);

            LeaderBoard leads = new LeaderBoard();
            leads = JsonUtility.FromJson<LeaderBoard>("{ \"playerScores\": " + request.downloadHandler.text + "}");
            
            responseText.text = "";
            place = 0;
            foreach(PlayerScore player in leads.playerScores)
            {
                place++;
                responseText.text += $"{place}. {player.name}: Score {player.score}; Time {player.playedTime:f1}s; \n";
            }
        }
    }
}
