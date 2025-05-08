using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor.Experimental.GraphView;

[Serializable]
public class ChatList
{
    public string[] mensajes;
}

public class DataGato : MonoBehaviour
{
    public List<string> users;

    public static event Action usernameUpdated;
    public static event Action usernameError;
    public static event Action noUsername;
    public static event Action GameInvite;
    public static event Action Accept;
    public static event Action Decline;

    WebSocket _websocket;

    public TMP_InputField IF_username;
    string _username;
    string _messageCode;
    string _messageMain;

    public GameObject buttonPrefab;
    public Transform contentContainer;

    public TMP_InputField messageInputField;
    public TextMeshProUGUI chatDisplay;
    private string selectedUser;


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
                    Debug.Log("_messageCode: " + _messageCode);
                    Debug.Log("_messageMain: " + _messageMain);
                    StartCoroutine(WaitforJson());
                    break;
                case "400":
                    GameInvite?.Invoke();
                    break;
                case "500":
                    Debug.Log("Send 500");
                    NewUser();
                    break;
                case "600":
                    chatDisplay.text += _messageMain + "\n";
                    break;

                case "601":
                    string[] history = JsonUtility.FromJson<ChatList>("{\"mensajes\":" + _messageMain + "}").mensajes;
                    chatDisplay.text = ""; // Limpiar chat antes de mostrar historial
                    foreach (string msg in history)
                    {
                        chatDisplay.text += msg + "\n";
                    }
                    break;
            }
        };
        await _websocket.Connect();
    }

    async public void NewUser()
    {
        await _websocket.SendText("300|");
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

    public void AcceptInvite()
    {
        Accept?.Invoke();
    }

    public void DeclineInvite()
    {
        Decline.Invoke();
    }

    IEnumerator WaitforJson()
    {
        while (_messageMain == null)
        {
            yield return new WaitForSeconds(.5f);
        }

        UserList usuarios = JsonUtility.FromJson<UserList>(_messageMain);
        GenerateUserButtons(usuarios.users);
    }

    void GenerateUserButtons(List<string> users)
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (string user in users)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, contentContainer);
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = user;

            Button button = buttonObj.GetComponent<Button>();
            string selectedUser = user;

            button.onClick.AddListener(() => OnUserButtonClicked(selectedUser));
        }
    }

    //async void OnUserButtonClicked(string username)
    //{
    //    Debug.Log("Selected User: " + username);
    //    await _websocket.SendText("600|" + username + "|Mensaje Desde Unity");
    //}

    public async void SendMessageToUser()
    {
        if (!string.IsNullOrEmpty(selectedUser) && !string.IsNullOrEmpty(messageInputField.text))
        {
            string messageToSend = messageInputField.text;
            await _websocket.SendText("600|" + selectedUser + "|" + messageToSend);
            messageInputField.text = ""; // limpiar el input
        }
    }

    async void OnUserButtonClicked(string username)
    {
        selectedUser = username;
        Debug.Log("Selected User: " + selectedUser);

        await _websocket.SendText("601|" + selectedUser); // Solicita historial
    }
}
