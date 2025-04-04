﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Check : MonoBehaviour
{
    public static event Action<int> OnPress;

    GatoWS _gatoWS;

    public Button button;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI ActualText;

    public string playerSide;
    string MyID;

    public int _actual = 1;

    public int checkPosition;

    private GameManager gameManager;

    SetID setID;
    //phpManager phpManager;

    private void Start()
    {
        SetID.SetIDGame += SetMyID;
        ActualText.text = "Turno: " + _actual.ToString();
    }

    private void OnEnable()
    {
        GatoWS.UpdateActual += ChangeTurn;
    }
    private void OnDisable()
    {
        GatoWS.UpdateActual -= ChangeTurn;
    }

    void ChangeTurn()
    {
        _actual++;
        ActualText.text = "Turno: " + _actual.ToString();
    }

    public void setGameManagerReference(GameManager manager)
    {
        gameManager = manager;
    }

    public void setSpace()
    {
        if (MyID == "2" && _actual % 2 == 0)
        {
            buttonText.text = "O";
            button.interactable = false;
            OnPress.Invoke(checkPosition);
        }

        if (MyID == "1" && _actual % 2 != 0)
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
    }
    
}
