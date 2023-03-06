using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class Base_Server : MonoBehaviour
{
    public NetworkDriver driver;
    protected List<NetworkConnection> connections;
    public string ipAddress = "192.168.1.220";
    private ushort port = 8000;
    public int PlayerLimit = 4;
    public bool serverActive = false;
    public List<int> playerList = new List<int>();
    GameDataFile gameDataFile;

//#if UNITY_EDITOR
    private void Start()
    {
        gameDataFile = FindObjectOfType<GameDataFile>();
        if (gameDataFile.serverIpAddress == "") ipAddress = GetLocalIPAddress();
        else ipAddress = gameDataFile.serverIpAddress;
        //#if UNITY_EDITOR
        //    ipAddress = "192.168.1.220";
        //#endif

        Init(); 
    }
    private void Update()
    { UpdateServer(); }
    private void OnDestroy()
    { Shutdown(); }
//#endif

    public virtual void Init()
    {
        //Initialise driver
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint = NetworkEndPoint.Parse(ipAddress, port);
        //NetworkEndPoint endpoint = NetworkEndPoint.LoopbackIpv4;
        //endpoint.Port = port; //Make sure port isnt used (dont use 80 or 443)
        if (driver.Bind(endpoint) != 0) { Debug.Log("There was an Error binding to port " + endpoint.Port); }
        else { driver.Listen(); }

        //Initialise connection list
        connections = new List<NetworkConnection>(PlayerLimit);

        //mark as active
        serverActive = true;
    }
    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete();
        CleanUpConnections();
        AcceptNewConnections();
        UpdateMesssagePump();
    }
    public virtual void Shutdown()
    {
        driver.Dispose();
        connections.Clear();
        Debug.Log("Server shutdown");

        //mark as inactive
        serverActive = false;
    }
    private void CleanUpConnections()
    {
        for(int i = 0; i < connections.Count; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.Remove(connections[i]);
                --i;
            }
        }
    }
    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Accepted a connection");
            BroadcastPlayerId();
        }
    }
    protected virtual void UpdateMesssagePump()
    {
        DataStreamReader stream;
        for(int i = 0; i < connections.Count; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if(cmd == NetworkEvent.Type.Data)
                { OnData(stream); }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    BroadcastPlayerId();
                }
            }
        }
    }
    public virtual void OnData(DataStreamReader stream)
    {
        NetMessage msg = null;
        var opCode = (OpCode)stream.ReadByte();
        switch (opCode)
        {
            case OpCode.CHAT_MESSAGE: msg = new Net_ChatMessage(stream); break;
            case OpCode.PLAYER_POSITION: msg = new Net_PlayerPosition(stream); break;
            case OpCode.PLAYER_ANIM: msg = new Net_PlayerAnim(stream); break;
            case OpCode.PLAYER_INFO: msg = new Net_PlayerInfo(stream); break;
            default:
                Debug.Log("Recieved data has no OpCode");
                break;
        }
        msg.RecievedOnServer(this);

        //byte opCode = stream.ReadByte();
        //FixedString128Bytes chatMessage = stream.ReadFixedString128();
        //Debug.Log("Got " + opCode + " from the client");
        //Debug.Log(chatMessage);
    }
    public virtual void Broadcast(NetMessage msg)
    {
        for(int i = 0; i < connections.Count; i++)
        { if (connections[i].IsCreated) SendToClient(connections[i], msg); }
    }
    public virtual void BroadcastPlayerId()
    {
        playerList.Clear();
        //Give the playerID of each client
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].IsCreated)
            {
                NetMessage msg = new Net_PlayerId(i);
                SendToClient(connections[i], msg);
                playerList.Add(i);
            }
        }
    }
    public virtual void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }
    public static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
