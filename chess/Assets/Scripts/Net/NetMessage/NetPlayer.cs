using Unity.Networking.Transport;
using UnityEngine;
public class NetPlayer : NetMessage
{
    public string matchName;
    public NetPlayer() {
        Code = OpCode.PLAYER;
    }

    public NetPlayer(DataStreamReader reader) {
        Code = OpCode.PLAYER;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
        writer.WriteFixedString64(matchName);
    }
    public override void Deserialize(ref DataStreamReader reader) {
        matchName = reader.ReadFixedString64().ToString();
    }

    public override void ReceivedOnClient() {
        NetUtility.C_PLAYER?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn) {
        NetUtility.S_PLAYER?.Invoke(this, cnn);
    }
}
