using UnityEngine;
using NativeWebSocket;

public class GatoWS : MonoBehaviour
{
    WebSocket _websocket;
    private SetID _setId;

    private int[] _board = new int[9];
    
    public string strMessage = "Vlad";

    void OnEnable()
    {
        Check.OnPress += GetActual;
    }

    void OnDisable()
    {
        Check.OnPress -= GetActual;
    }
    
    // Start is called before the first frame update
    async void Start()
    {
        _websocket = new WebSocket("ws://localhost:8080");

        _websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            Init(1);
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
            //Debug.Log("OnMessage!");
            //Debug.Log(bytes);

            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + message);

            if (message.StartsWith("Tablero actualizado:"))
            {
                string boardData = message.Replace("Tablero actualizado: ", "").Trim();
                string[] values = boardData.Split(',');

                for (int i = 0; i < values.Length; i++)
                {
                    _board[i] = int.Parse(values[i]); // Guardar en la variable local
                }

                Debug.Log("Tablero actualizado en Unity: " + string.Join(",", _board));
            }
        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        await _websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _websocket.DispatchMessageQueue(); // Mandar mensajes sin enviar
#endif
    }

    public void Send()
    {
        SendWebSocketMessage();
    }

    async void SendWebSocketMessage()
    {
        if (_websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            //await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await _websocket.SendText("Plaintext message: "+ strMessage);
        }
    }

    private async void OnApplicationQuit()
    {
        await _websocket.Close();
    }

    public async void Init(int TurnoActual)
    {
        if (_websocket.State == WebSocketState.Open)
        {
            await _websocket.SendText("201|");
        }
    }

    public async void GetActual(int tablero)
    {
        if (_websocket.State == WebSocketState.Open)
        {
            await _websocket.SendText("202|" + tablero + "|" + _setId.MyID);
        }
    }
}
