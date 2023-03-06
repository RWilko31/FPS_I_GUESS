using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;


public class Net_PlayerAnim : NetMessage
{
    // 0 - 8 bits OP CODE
    public int PlayerId { set; get; }
    public int Jump { set; get; }
    public int DJump { set; get; }
    public int Crouch { set; get; }
    public int Prone { set; get; }
    public int SStop { set; get; }
    public float X { set; get; }
    public float LastxyMove { set; get; }

    public Net_PlayerAnim()
    {
        code = OpCode.PLAYER_ANIM;
    }
    public Net_PlayerAnim(DataStreamReader reader)
    {
        code = OpCode.PLAYER_ANIM;
        Deserialise(reader);
    }
    public Net_PlayerAnim(int playerId, bool jump, bool dJump, bool crouch, bool prone, bool sStop, float x, float lxy)
    {
        code = OpCode.PLAYER_ANIM;
        PlayerId = playerId;
        Jump = convertBoolToInt(jump);
        DJump = convertBoolToInt(dJump);
        Crouch = convertBoolToInt(crouch);
        Prone = convertBoolToInt(prone);
        SStop = convertBoolToInt(sStop);
        X = x;
        LastxyMove = lxy;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteInt(PlayerId);
        writer.WriteInt(Jump);
        writer.WriteInt(DJump);
        writer.WriteInt(Crouch);
        writer.WriteInt(Prone);
        writer.WriteInt(SStop);
        writer.WriteFloat(X);
        writer.WriteFloat(LastxyMove);
    }
    public override void Deserialise(DataStreamReader reader)
    {
        //The first byte is handled already
        PlayerId = reader.ReadInt();
        Jump = reader.ReadInt();
        DJump = reader.ReadInt();
        Crouch = reader.ReadInt();
        Prone = reader.ReadInt();
        SStop = reader.ReadInt();
        X = reader.ReadFloat();
        LastxyMove = reader.ReadFloat();
    }
    private int convertBoolToInt(bool i)
    {
        if (i == false) return 0;
        else return 1;
    }
    public override void RecievedOnServer(Base_Server server)
    {
        server.Broadcast(this);
    }
    public override void RecievedOnClient()
    {
        //Debug.Log("C: ID: " + PlayerId + " J: " + Jump.ToString() + " dJ: " + DJump.ToString() + " C: " + Crouch.ToString() + " P: " + Prone.ToString() + " S: " + SStop.ToString() + " X: " + X.ToString());
    }
    public override int RecievedId()
    {
        return PlayerId;
    }
    public override List<int> getPlayerAnimInts()
    {
        List<int> intList = new List<int>();
        intList.Add(PlayerId);
        intList.Add(Jump);
        intList.Add(DJump);
        intList.Add(Crouch);
        intList.Add(Prone);
        intList.Add(SStop);
        return intList;
    }
    public override List<float> getPlayerAnimFloats()
    {
        List<float> floatList = new List<float>();
        floatList.Add(X);
        floatList.Add(LastxyMove);
        return floatList;
    }
}
