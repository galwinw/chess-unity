using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;


public class Server : MonoBehaviour {
    #region Singleton implementation
    public static Server Instance {set; get;}

    private void Awake() {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private const float keepAliveTickRate = 20.0f;
    private float lastKeepAliveTime;

    public Action connectionDropped;

    // Methods 
    public void Init(ushort port) {
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = port;

        if(driver.Bind(endpoint) != 0) {
            Debug.Log("Failed to bind to port " + port.ToString());
            return;
        } else {
            driver.Listen();
            Debug.Log("Server started on port " + port.ToString());
        }
        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true;
    }

    public void Shutdown() {
        if (isActive) {
            driver.Dispose();
            connections.Dispose();
            isActive = false;
        }
    }

    public void OnDestroy() {
        Shutdown();
    }

    public void Update() {
        if (!isActive) {
            return;
        }

        KeepAlive();

        driver.ScheduleUpdate().Complete();

        CleanupConnections();
        AcceptNewConections();
        UpdateMessagePump();

    }

    private void KeepAlive() {
        if (Time.time - lastKeepAliveTime > keepAliveTickRate) {
            lastKeepAliveTime = Time.time;
            Broadcast(new NetKeepAlive());
        }
    }

    private void CleanupConnections() {
        for (int i = 0; i < connections.Length; i++) {
            if (!connections[i].IsCreated) {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    private void AcceptNewConections() {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection)) {
            connections.Add(c);
            Debug.Log("Accepted a connection");
        }
    }

    private void UpdateMessagePump() {
        DataStreamReader stream;
        for (int i = 0; i < connections.Length; i++) {
            NetworkEvent.Type cmd;
            
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty) {
                if (cmd == NetworkEvent.Type.Data) {
                    NetUtility.OnData(stream, connections[i], this);

                } else if (cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    Shutdown();//Since chess is a two player game we can shutdown when one leaves the server
                }
            }

        }
    }

    // Server Specific 
    public void SendToClient(NetworkConnection c, NetMessage msg) {
        DataStreamWriter writer;
        driver.BeginSend(c, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    public void Broadcast(NetMessage msg) {
        Debug.Log("got to server broadcasting");
        for (int i = 0; i < connections.Length; i++) {
            if (connections[i].IsCreated) {
                Debug.Log($"Sendingg {msg.Code} to {connections[i].InternalId}");
                SendToClient(connections[i], msg);
            }
            
        }
    }


}
