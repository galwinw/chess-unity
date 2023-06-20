using Unity.Networking.Transport;
using UnityEngine;

public class NetRematch : NetMessage {
    public int teamId;
    public byte wantRematch;

    public NetRematch() {
        Code = OpCode.REMATCH;
    }

    public NetRematch (DataStreamReader reader) {
        Code = OpCode.REMATCH;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamId);
        writer.WriteByte((byte)wantRematch);
    }

    public override void Deserialize(ref DataStreamReader reader) {
        teamId = reader.ReadInt();
        wantRematch = reader.ReadByte();
    }

    public override void ReceivedOnClient() {
        NetUtility.C_REMATCH?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn) {
        NetUtility.S_REMATCH?.Invoke(this, cnn);
    }
}
