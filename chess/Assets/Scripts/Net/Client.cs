using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;

public class Client : MonoBehaviour
{
    #region Singleton implementation
    public static Client Instance {set; get;}

    private void Awake() {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;
    
    public Action connectionDropped = null;

    public void Init(String ip, ushort port) {
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, port);

        Debug.Log("Attempting to connect to Server on " + endpoint.Address);
        connection = driver.Connect(endpoint);
        isActive = true;

        RegisterToEvent();
    }

    public void Shutdown() {
        if (isActive) {
            UnregisterToEvent();
            isActive = false;
            driver.Dispose();
            connection = default(NetworkConnection);
        }
    }

    public void OnDestroy() {
        Shutdown();
    }

    public void Update() {
        if (!isActive) {
            return;
        }

        driver.ScheduleUpdate().Complete();
        CheckAlive();

        UpdateMessagePump();
    }

    private void CheckAlive() {
        if (!connection.IsCreated && isActive) {
            Debug.Log("Connection to server lost");
            connectionDropped?.Invoke();
            Shutdown();
        }
    }


    private void UpdateMessagePump() {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty ) {
            if (cmd == NetworkEvent.Type.Connect) {
                Debug.Log("Connected to server");
                //SendToServer(new NetWelcom());
            } else if (cmd == NetworkEvent.Type.Data) {
                NetUtility.OnData(stream, default(NetworkConnection));
                
            } else if (cmd == NetworkEvent.Type.Disconnect) {
                Debug.Log("Disconnected from server");
                connection = default(NetworkConnection);
                connectionDropped?.Invoke();
                Shutdown();
            }
        }

    }


    public void SendToServer(NetMessage message){
        if (!isActive) {
            return;
        }

        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        message.Serialize(ref writer);
        driver.EndSend(writer);
    }

    // Event Parsing 
    private void RegisterToEvent() {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
    }

    private void UnregisterToEvent() {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage nm) {
        // Send it back, to keep both side alive 
        SendToServer(nm);
    }

}
