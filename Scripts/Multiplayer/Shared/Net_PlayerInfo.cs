using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class Net_PlayerInfo: NetMessage
{
    // 0 - 8 bits OP CODE
    public int PlayerId { set; get; }
    public string Weapon { set; get; }

    public Net_PlayerInfo()
    {
        code = OpCode.PLAYER_INFO;
    }
    public Net_PlayerInfo(DataStreamReader reader)
    {
        code = OpCode.PLAYER_INFO;
        Deserialise(reader);
    }
    public Net_PlayerInfo(int playerId, string weapon)
    {
        code = OpCode.PLAYER_INFO;
        PlayerId = playerId;
        Weapon = weapon;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteInt(PlayerId);
        writer.WriteFixedString128(Weapon);
    }
    public override void Deserialise(DataStreamReader reader)
    {
        //The first byte is handled already
        PlayerId = reader.ReadInt();
        Weapon = reader.ReadFixedString128().ToString();
    }
    public override void RecievedOnServer(Base_Server server)
    {
        server.Broadcast(this);
    }
    public override void RecievedOnClient()
    {
        //Debug.Log("C: ID: " + PlayerId + " Weapon: " + Weapon);
    }
    public override int RecievedId()
    {
        return PlayerId;
    }
    public override string getPlayerWeapon()
    {
        return Weapon;
    }
}
