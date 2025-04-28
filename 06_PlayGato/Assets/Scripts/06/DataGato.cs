using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class UsersList
{
    public List<string> users;
}

public class DataGato : MonoBehaviour
{
    public List<string> users;

    public static event Action usernameUpdated;
    public static event Action usernameError;
    public static event Action noUsername;
    public static event Action<string> OnUserButtonClicked;

    WebSocket _websocket;

    public TMP_InputField IF_username;
    string _username;
    string _messageCode;

    [Header("UI Elements")]
    public GameObject buttonPrefab;
    public Transform buttonContainer;

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
            }

            if (message.StartsWith("300|"))
            {
                string json = message.Substring(4);
                UsersList usuarios = JsonUtility.FromJson<UsersList>(message);
                foreach (string user in usuarios.users)
                {
                    Debug.Log("Usuario disponible: " + user);
                }
            }
            else
            {
                Debug.Log("Mensaje normal recibido: " + message);
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

    void GenerateUserButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (string user in users)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = user;

            string capturedUser = user;
            newButton.GetComponent<Button>().onClick.AddListener(() => OnUserButtonClicked(capturedUser));
        }
    }
}
