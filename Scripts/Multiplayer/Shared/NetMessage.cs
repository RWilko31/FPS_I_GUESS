using UnityEngine;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class NetMessage
{
    public OpCode code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer) { }
    public virtual void Deserialise(DataStreamReader reader) { }
    public virtual void RecievedOnClient() { }
    public virtual void RecievedOnServer(Base_Server server) { }

    //PlayerID
    public virtual int RecievedId() { return 0; }
    //Positions
    public virtual Vector3 getPlayerPos() { return Vector3.zero; }
    public virtual Vector3 getPlayerRot() { return Vector3.zero; }
    //Animation
    public virtual List<int> getPlayerAnimInts() { return new List<int>(); }
    public virtual List<float> getPlayerAnimFloats() { return new List<float>(); }
    public virtual string getPlayerWeapon() { return ""; }
}
