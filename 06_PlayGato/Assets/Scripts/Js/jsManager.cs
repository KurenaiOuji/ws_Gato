using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class jsManager : MonoBehaviour
{
    JS_DataBase js_DataBase;
    SetID setID;

    int position;
    string MyID;

    public Button[] Board;

    void Awake()
    {
        Check.OnPress += SetAction;
        SetID.SetIDGame += ID;
    }

    private void Start()
    {
        StartCoroutine(StartDb());
    }

    void SetAction(int boardPosition)
    {
        position = boardPosition;
        StartCoroutine(SetPlay());
    }

    void ID(string IDGame)
    {
        MyID = IDGame;
    }

    IEnumerator StartDb()
    {
        using UnityWebRequest GameStart = UnityWebRequest.Get("http://localhost:8080/action/init");
        yield return GameStart.SendWebRequest();

        if (GameStart.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Game Started Correctly: " + GameStart.downloadHandler.text);

            StartCoroutine(GetDataBase());
        }
        else
        {
            Debug.LogError("Error, Game Couldn't Start: " + GameStart.error);
        }
    }

    IEnumerator GetDataBase()
    {
        while (true)
        {
            using UnityWebRequest JsDataBase = UnityWebRequest.Get("http://localhost:8080/action/data");
            {
                yield return JsDataBase.SendWebRequest();

                if (JsDataBase.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("JSON recibido: " + JsDataBase.downloadHandler.text);

                    // ✅ Base de datos actualizada
                    js_DataBase = JsonUtility.FromJson<JS_DataBase>(JsDataBase.downloadHandler.text);

                    StartCoroutine(GetStatus());
                }
                else
                {
                    Debug.LogError("Error al descargar JSON: " + JsDataBase.error);
                }

                yield return new WaitForSeconds(2);
            }
        }
    }

    IEnumerator SetPlay()
    {
        using UnityWebRequest SetPlay = UnityWebRequest.Get("http://localhost:8080/turn/" + MyID + "/" + position);
        yield return SetPlay.SendWebRequest();
        Debug.Log("Play Sent");
    }

    IEnumerator GetStatus()
    {
        while (true)
        {
            while (js_DataBase.board == null)
            {
                Debug.Log("⏳ Esperando datos de JS...");
                yield return new WaitForSeconds(1);
            }

            Debug.Log("✅ Datos recibidos, actualizando tablero...");
            SetStatusBoard();
            yield return new WaitForSeconds(2f);
        }
    }

    public void SetStatusBoard()
    {
        if (js_DataBase.board == null)
        {
            Debug.LogError("❌ ERROR: Intentando actualizar pero 'board' es null.");
            return;
        }

        Debug.Log("✅ Actualizando tablero con: " + string.Join(", ", js_DataBase.board));

        for (int arrayPos = 0; arrayPos < Board.Length && arrayPos < js_DataBase.board.Length; arrayPos++)
        {
            if (Board[arrayPos] == null)
            {
                Debug.LogError($"❌ ERROR: Botón en posición {arrayPos} es null. Verifica el Inspector.");
                continue;
            }

            if (js_DataBase.board[arrayPos] == 1)
            {
                Board[arrayPos].GetComponentInChildren<TextMeshProUGUI>().text = "X";
                Board[arrayPos].interactable = false;
            }
            else if (js_DataBase.board[arrayPos] == 2)
            {
                Board[arrayPos].GetComponentInChildren<TextMeshProUGUI>().text = "O";
                Board[arrayPos].interactable = false;
            }
        }
        Debug.Log("✅ Tablero actualizado correctamente.");
    }
}
