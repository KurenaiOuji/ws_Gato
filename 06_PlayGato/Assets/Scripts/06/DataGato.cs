using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.UI;
using TMPro;
using System;

public class DataGato : MonoBehaviour
{
    public List<string> users;

    public static event Action usernameUpdated;
    public static event Action usernameError;
    public static event Action noUsername;

    WebSocket _websocket;

    public TMP_InputField IF_username;
    string _username;
    string _messageCode;
    string _messageMain;

    public GameObject buttonPrefab;
    public Transform contentContainer;

    void Start()
    {
        WebSocketSystem(); 
    }

    void Update()
    {
        if (_websocket != null)
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
            _websocket.DispatchMessageQueue();
            #endif
        }
    }


    async void WebSocketSystem()
    {
        _websocket = new WebSocket("ws://localhost:8080");

        _websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        _websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        _websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        _websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);

            Debug.Log("System: " + message);

            string[] parts = message.Split("|");

            if (parts.Length > 0)
            {
                _messageCode = parts[0];
                _messageMain = parts[1];
            }

            switch (_messageCode)
            {
                case "201":
                    Debug.Log("System: Success! | 201");
                    usernameUpdated?.Invoke();
                    break;
                case "202":
                    Debug.Log("System: Username allready exist! | 202");
                    usernameError?.Invoke();
                    break;
                case "301":
                    UserList usuarios = JsonUtility.FromJson<UserList>(_messageMain);
                    GenerateUserButtons(usuarios.users);

                    break;
            }
        };
        await _websocket.Connect();
    }

    async public void ChangeUsername()
    {
        _username = IF_username.text;
        if(_username == "")
        {
            Debug.Log("System: Please enter a valid Username");
            noUsername?.Invoke();
        }
        else
        {
            await _websocket.SendText("200|" + _username);
            await _websocket.SendText("300|");
        }
    }
    void GenerateUserButtons(List<string> users)
    {
        // Crear un botón por cada usuario
        foreach (string user in users)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, contentContainer);
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = user;

            Button button = buttonObj.GetComponent<Button>();
            string selectedUser = user; // Necesario para capturar correctamente el nombre en el lambda

            button.onClick.AddListener(() => OnUserButtonClicked(selectedUser));
        }
    }

    async void OnUserButtonClicked(string username)
    {
        Debug.Log("Selected User: " + username);
        await _websocket.SendText("401|" + username);
    }
}
