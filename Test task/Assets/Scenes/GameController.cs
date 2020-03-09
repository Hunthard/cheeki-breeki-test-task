using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int num;
    public float timer, cooldown;
    public string answer;
    public Text QuestionText;
    public Text ResultText;

    public bool getAnswer;

    public Button buttonCheeki;
    public Button buttonBreeki;
    public Button buttonNumber;
    public Button buttonCheekiBreeki;

    public Image backgroundImage;
    public Image defeatImage;
    public Slider timeSlider;

    private const string URL = "https://localhost:5001/api/leaderboard";

    void Start()
    {
        num = 1;
        getAnswer = true;
        timeSlider.value = cooldown;
        backgroundImage.gameObject.SetActive(true);
        defeatImage.gameObject.SetActive(false);
    }

    void Update()
    {
        GetQuestion();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void CorrectAnswer()
    {
        num++;
        backgroundImage.color = Color.green;
        buttonBreeki.enabled = false;
        buttonCheeki.enabled = false;
        buttonCheekiBreeki.enabled = false;
        buttonNumber.enabled = false;

        timer = 0;
        timeSlider.value = cooldown;

        if (num <= 100)
        {
            GetQuestion();
        }
        else
        {
            FinishGame(true);
            ResultText.text = $"Win \n You score: {num - 1} \n Time: {Time.timeSinceLevelLoad:f1} sec";
        }
    }

    public void FinishGame(bool state)
    {
        if (state)
            backgroundImage.color = Color.green;
        else
            backgroundImage.color = Color.red;

        buttonBreeki.enabled = false;
        buttonCheeki.enabled = false;
        buttonCheekiBreeki.enabled = false;
        buttonNumber.enabled = false;

        ResultText.text = $"Defeat \n You score: {num - 1} \n Time: {Time.timeSinceLevelLoad:f1} sec";

        StartCoroutine(delay());

        StartCoroutine(SendScore());

        enabled = false;
    }

    public IEnumerator delay()
    {
        yield return new WaitForSeconds(0.5f);

        backgroundImage.gameObject.SetActive(false);
        defeatImage.gameObject.SetActive(true);
    }

    public void GetQuestion()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timeSlider.value -= Time.deltaTime;
        }
        else
        {
            if (getAnswer)
            {
                QuestionText.text = DevideNum(num);
                timer = cooldown;
                backgroundImage.color = Color.white;

                buttonBreeki.enabled = true;
                buttonCheeki.enabled = true;
                buttonCheekiBreeki.enabled = true;
                buttonNumber.enabled = true;
                getAnswer = false;
            }  
            else
            {
                FinishGame(false);
            }
        }
    }
    
    public void Answer(Text ans)
    {        
        getAnswer = true;

        if (string.Equals(answer, ans.text))
        {
            CorrectAnswer();
        }
        else
        {
            FinishGame(false);
        }

        timer = cooldown;
    }

    public IEnumerator SendScore()
    {
        PlayerScore ps = new PlayerScore(num, "Player", num - 1, Time.timeSinceLevelLoad);
        string myData = JsonUtility.ToJson(ps);

        var postRequest = new UnityWebRequest(URL, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(myData);
        postRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        postRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        postRequest.SetRequestHeader("Content-Type", "application/json");

        yield return postRequest.SendWebRequest();

        if (postRequest.isNetworkError)
        {
            Debug.Log("Error While Sending: " + postRequest.error);
        }
        else
        {
            Debug.Log("Received: " + postRequest.downloadHandler.text);
        }
    }

    public string DevideNum(int i)
    {
        if (i % 3 == 0)
        {
            if (i % 5 == 0)
            {
                answer = "CheekiBreeki";
                return i.ToString() + " - " + answer;
            }
            else
            {
                answer = "Cheeki";
                return i.ToString() + " - " + answer;
            }
        }
        if (i % 5 == 0)
        {
            answer = "Breeki";
            return i.ToString() + " - " + answer;
        }
        else
        {
            answer = "Number";
            return i.ToString() + " - " + answer;
        }
    }
}
