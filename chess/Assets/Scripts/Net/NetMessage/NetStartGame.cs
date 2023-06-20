using Unity.Networking.Transport;


public class NetStartGame : NetMessage {
    

    public NetStartGame() {
        Code = OpCode.START_GAME;
    }

    public NetStartGame (DataStreamReader reader) {
        Code = OpCode.WELCOME;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
        
    }

    public override void Deserialize(ref DataStreamReader reader) {
        
    }

    public override void ReceivedOnClient() {
        NetUtility.C_START_GAME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn) {
        NetUtility.S_START_GAME?.Invoke(this, cnn);
    }
}