using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Check : MonoBehaviour
{
    public static event Action<int> OnPress;

    public Button button;
    public TextMeshProUGUI buttonText;
    public string playerSide;
    string MyID;

    public int checkPosition;

    private GameManager gameManager;

    JS_DataBase js_DataBase;
    SetID setID;
    //phpManager phpManager;

    private void Start()
    {
        StartCoroutine(GetDataBasejs());
        SetID.SetIDGame += SetMyID;
    }

    public void setGameManagerReference(GameManager manager)
    {
        gameManager = manager;
    }

    public void setSpace()
    {
        if (MyID == "id2" && js_DataBase.actual == 2)
        {
            buttonText.text = "O";
            button.interactable = false;
            OnPress.Invoke(checkPosition);
        }

        if (MyID == "id1" && js_DataBase.actual == 1)
        {
            buttonText.text = "X";
            button.interactable = false;
            OnPress.Invoke(checkPosition);
        }
    }

    void SetMyID(string setmyID)
    {
        MyID = setmyID;
        print("Ckeck ID: " + MyID);
        Debug.Log("JS_DataBase Actual: " + js_DataBase.actual);
    }

    IEnumerator GetDataBasejs()
    {
        using UnityWebRequest JsDataBase = UnityWebRequest.Get("http://localhost:8080/action/data");
        {
            yield return JsDataBase.SendWebRequest();

            if (JsDataBase.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log("JSON recibido: " + JsDataBase.downloadHandler.text);

                // ✅ Base de datos actualizada
                js_DataBase = JsonUtility.FromJson<JS_DataBase>(JsDataBase.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al descargar JSON: " + JsDataBase.error);
            }
        }
    }
}
