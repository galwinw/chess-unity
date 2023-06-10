using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;


public enum OpCode {
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    MAKE_MOVE = 4,
    REMATHC = 5,
}

public class NetMessage : MonoBehaviour
{
    public OpCode Code { set; get;}

    public virtual void Serialize(ref DataStreamWriter writer) {
        writer.WriteInt((int)Code);
    }

    public virtual void Deserialize(ref DataStreamReader reader) {
        Code = (OpCode)reader.ReadInt();
    }

    public virtual void ReceivedOnClient() {

    }

    public virtual void ReceivedOnServer(NetworkConnection cnn) {

    }

}

