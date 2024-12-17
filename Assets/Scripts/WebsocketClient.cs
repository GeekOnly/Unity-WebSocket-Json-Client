using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Net.WebSockets;
using Newtonsoft.Json;

public class WebsocketClient : MonoBehaviour
{
    private ClientWebSocket webSocket = null;
    private Uri serverUri = new Uri("ws://localhost:8080/ws");

    [SerializeField]
    private Dictionary<string, bool> platformState = new Dictionary<string, bool>();

    [SerializeField]
    private ObjectsPlatfomerManager objectsPlatfomerManager = null;

    private CancellationTokenSource cancellation = new CancellationTokenSource();

    private async void Start()
    {
        await ConnectToServer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            platformState["platformer1"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            platformState["platformer2"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            platformState["platformer3"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            platformState["platformer4"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            platformState["platformer5"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            platformState["platformer6"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            platformState["platformer7"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            platformState["platformer8"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            platformState["platformer9"] = true;
            SendPlatformUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            platformState["platformer10"] = true;
            SendPlatformUpdate();
        }

    }

    private async Task ConnectToServer()
    {
        webSocket = new ClientWebSocket();
        try
        {
            Debug.Log("Connecting to WebSocket server...");
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("Connected to WebSocket server!");

            // Start receiving messages
            await ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError($"WebSocket connection error: {ex.Message}");
        }
    }

    private async Task ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellation.Token);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log($"Received: {message}");

                    UpdatePlatformStateFromServer(message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("WebSocket receive operation canceled.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"WebSocket receive error: {ex.Message}");
        }
    }

    private void UpdatePlatformStateFromServer(string message)
    {
        try
        {
            Debug.Log($"{message}");
            platformState = JsonConvert.DeserializeObject<Dictionary<string, bool>>(message);

            // Update platform states in the game
            foreach (var platformEntry in platformState)
            {
                Debug.Log($"Updated platform '{platformEntry.Key}'");
                foreach (var platformActor in objectsPlatfomerManager.objects)
                {
                    if (platformActor.NamePlatfomer == platformEntry.Key)
                    {
                        platformActor.isActive = platformEntry.Value;
                        Debug.Log($"Updated platform '{platformActor.NamePlatfomer}' isActive to {platformEntry.Value}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse or update platform state: {ex.Message}");
        }
    }

    private async void SendPlatformUpdate()
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            Debug.LogError("WebSocket is not connected. Cannot send platform update.");
            return;
        }

        try
        {
            string jsonData = JsonConvert.SerializeObject(platformState);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData);

            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.Log("Sent platform state update to server.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending platform update: {ex.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        cancellation.Cancel();
        webSocket?.Dispose();
    }
}
