using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI[] textList;
    public TextMeshProUGUI gameOverText;

    private string playerSide;
    private string playerID;
    
    JS_DataBase gameData;
    SetID setID;

    int turns = 0;

    private void Awake()
    {
        SetGameManagerReferenceOnButtons();

        SetID.SetIDGame += SetMySide;

        
    }

    private void Start()
    {
        //StartCoroutine(GetStatus());
    }

    void SetGameManagerReferenceOnButtons()
    {
        for (int i = 0; i < textList.Length; i++)
        {
            //textList[i].GetComponentInParent<Check>().setGameManagerReference(this);
        }
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }

    public void EndTurn()
    {
        turns++;

        if (textList[0].text == playerSide && textList[1].text == playerSide && textList[2].text == playerSide)
        {
            GameOver();
        }

        if (textList[3].text == playerSide && textList[4].text == playerSide && textList[5].text == playerSide)
        {
            GameOver();
        }

        if (textList[6].text == playerSide && textList[7].text == playerSide && textList[8].text == playerSide)
        {
            GameOver();
        }

        if (textList[0].text == playerSide && textList[3].text == playerSide && textList[6].text == playerSide)
        {
            GameOver();
        }

        if (textList[1].text == playerSide && textList[4].text == playerSide && textList[7].text == playerSide)
        {
            GameOver();
        }

        if (textList[2].text == playerSide && textList[5].text == playerSide && textList[8].text == playerSide)
        {
            GameOver();
        }

        if (textList[0].text == playerSide && textList[4].text == playerSide && textList[8].text == playerSide)
        {
            GameOver();
        }

        if (textList[2].text == playerSide && textList[4].text == playerSide && textList[6].text == playerSide)
        {
            GameOver();
        }

        //ChangeSides();

        if (turns >= 9)
        {
            gameOverText.gameObject.SetActive(true);

            gameOverText.text = "It's a Draw!!!";
        }
    }

    void GameOver()
    {
        for(int i = 0; i < textList.Length; i++)
        {
            textList[i].GetComponentInParent<Button>().interactable = false;
        }

        gameOverText.gameObject.SetActive(true);
        gameOverText.text = playerSide + " WINS!!!";
    }
/*
    void ChangeSides()
    {
        playerSide = (playerSide == "X") ? "O" : "X";
    }*/

    /*IEnumerator GetStatus()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/gato/gato.php?action=1");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            //Show Result as Text
            Debug.Log(www.downloadHandler.text);

            gameData = JsonUtility.FromJson<GatoDb>(www.downloadHandler.text);

            Debug.Log("Ronda: " + gameData.round);
            Debug.Log("Jugador actual: " + gameData.actual);
            Debug.Log("Score1: " + gameData.score1);
            Debug.Log("Score2: " + gameData.score2);
            Debug.Log("Jugador 1: " + gameData.p1);
            Debug.Log("Jugador 2: " + gameData.p2);
            Debug.Log("Board: " + string.Join(", ", gameData.board));

            //Or Retrive Results as Binary Data
            byte[] results = www.downloadHandler.data;
        }
    }*/
    
    IEnumerator TrowStatus()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/gato/gato.php?action=2&p1=1");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            //Show Result as Text
            Debug.Log(www.downloadHandler.text);

            //Or Retrive Results as Binary Data
            byte[] results = www.downloadHandler.data;
        }
    }

    IEnumerator Throw()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/gato/gato.php?action=3&id=id2&pos=4");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }

        else
        {
            //Show Result as Text
            Debug.Log(www.downloadHandler.text);

            //Or Retrive Results as Binary Data
            byte[] results = www.downloadHandler.data;
        } 
    }

    void SetMySide(string MyID)
    {
        playerID = MyID;

        if (playerID == "id1")
        {
            playerSide = "X";
        }

        else { playerSide = "O"; }

        print(playerSide);
    }
}
