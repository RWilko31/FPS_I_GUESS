using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class Net_PlayerPosition : NetMessage
{
    // 0 - 8 bits OP CODE
    public int PlayerId { set; get; }
    public float PositionX { set; get; }
    public float PositionY { set; get; }
    public float PositionZ { set; get; }
    public float RotationX { set; get; }
    public float RotationY { set; get; }
    public float RotationZ { set; get; }
    
    public Net_PlayerPosition()
    { 
        code = OpCode.PLAYER_POSITION; 
    }
    public Net_PlayerPosition(DataStreamReader reader)
    {
        code = OpCode.PLAYER_POSITION;
        Deserialise(reader);
    }
    public Net_PlayerPosition(int playerId, float px, float py, float pz, float rx, float ry, float rz)
    {
        code = OpCode.PLAYER_POSITION;
        PlayerId = playerId;
        PositionX = px;
        PositionY = py;
        PositionZ = pz;
        RotationX = rx;
        RotationY = ry;
        RotationZ = rz;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteInt(PlayerId);
        writer.WriteFloat(PositionX);
        writer.WriteFloat(PositionY);
        writer.WriteFloat(PositionZ);
        writer.WriteFloat(RotationX);
        writer.WriteFloat(RotationY);
        writer.WriteFloat(RotationZ);
    }
    public override void Deserialise(DataStreamReader reader)
    {
        //The first byte is handles already
        PlayerId = reader.ReadInt();
        PositionX = reader.ReadFloat();
        PositionY = reader.ReadFloat();
        PositionZ = reader.ReadFloat();
        RotationX = reader.ReadFloat();
        RotationY = reader.ReadFloat();
        RotationZ = reader.ReadFloat();
    }
    public override void RecievedOnServer(Base_Server server)
    {
        //Debug.Log("CLIENT::" + PlayerId + ":" + PositionX + "," + PositionY + "," + PositionZ + ":" + RotationX + "," + RotationY + "," + RotationZ);
        server.Broadcast(this);
    }
    public override void RecievedOnClient()
    {
        //Debug.Log("CLIENT::" + PlayerId + ":" + PositionX + "," + PositionY + "," + PositionZ + ":" +RotationX + "," + RotationY + "," + RotationZ);
    }
    public override int RecievedId()
    {
        return PlayerId;
    }
    public override Vector3 getPlayerPos()
    {
        return new Vector3(PositionX, PositionY, PositionZ);
    }
    public override Vector3 getPlayerRot()
    {
        return new Vector3(RotationX, RotationY, RotationZ);
    }
}
