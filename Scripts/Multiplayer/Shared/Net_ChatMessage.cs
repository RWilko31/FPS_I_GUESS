using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class Net_ChatMessage : NetMessage
{
    // 0 - 8 bits OP CODE
    public FixedString128Bytes ChatMessage { set; get; }

    public Net_ChatMessage()
    {
        code = OpCode.CHAT_MESSAGE;
    }
    public Net_ChatMessage(DataStreamReader reader)
    {
        code = OpCode.CHAT_MESSAGE;
        Deserialise(reader);
    }
    public Net_ChatMessage(string msg)
    {
        code = OpCode.CHAT_MESSAGE;
        ChatMessage = msg;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
        writer.WriteFixedString128(ChatMessage);
    }
    public override void Deserialise(DataStreamReader reader)
    {
        //The first byte is handles already
        ChatMessage = reader.ReadFixedString128();
    }
    public override void RecievedOnServer(Base_Server server)
    {
        Debug.Log("SERVER::" + ChatMessage);
        server.Broadcast(this);
    }
    public override void RecievedOnClient()
    {
        Debug.Log("CLIENT::" + ChatMessage);
    }
}
