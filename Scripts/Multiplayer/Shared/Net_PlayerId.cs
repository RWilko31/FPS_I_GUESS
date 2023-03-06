using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class Net_PlayerId : NetMessage
{
    // 0 - 8 bits OP CODE
    public int PlayerId { set; get; }
    public Net_PlayerId()
    {
        code = OpCode.PLAYER_POSITION;
    }
    public Net_PlayerId(DataStreamReader reader)
    {
        code = OpCode.PLAYER_POSITION;
        Deserialise(reader);
    }
    public Net_PlayerId(int playerId)
    {
        code = OpCode.PLAYER_ID;
        PlayerId = playerId;
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteInt(PlayerId);
    }
    public override void Deserialise(DataStreamReader reader)
    {
        //The first byte is handles already
        PlayerId = reader.ReadInt();
    }
    public override void RecievedOnServer(Base_Server server)
    {
        //Debug.Log("SERVER::" + PlayerId + ":" + PositionX + "," + PositionY + "," + PositionZ);
        server.Broadcast(this);
    }
    public override void RecievedOnClient()
    {
        Debug.Log(PlayerId);
        //Debug.Log("CLIENT::" + PlayerId + ":" + PositionX + "," + PositionY + "," + PositionZ);
    }
    public override int RecievedId()
    {
        return PlayerId;
    }
}
