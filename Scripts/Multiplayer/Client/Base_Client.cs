using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class Base_Client : MonoBehaviour
{
    public NetworkDriver driver;
    protected NetworkConnection connection;
    public string ipAddress;
    private ushort port = 8000;
    public bool connectedToServer = false;
    public List<int> playerList = new List<int>();

    private GameDataFile gameDataFile;
    private SendPlayerData sendPlayerData;
    private void Awake()
    {
        gameDataFile = FindObjectOfType<GameDataFile>();
        sendPlayerData = this.transform.GetComponent<SendPlayerData>();
        ipAddress = gameDataFile.serverIpAddress;
        if (ipAddress == "") 
        {
            ipAddress = GetLocalIPAddress();
            #if UNITY_EDITOR
            ipAddress = "192.168.1.220";
            #endif
        }
    }


    private void Start()
    { Init(); }
    private void Update()
    { UpdateServer(); }
    private void OnDestroy()
    { Shutdown(); }


    public virtual void Init()
    {
        //Initialise driver
        driver = NetworkDriver.Create();
        connection = default(NetworkConnection);
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        if (NetworkEndPoint.TryParse(ipAddress, port, out NetworkEndPoint endPoint))
        {
            connection = driver.Connect(endPoint);
        }
        //NetworkEndPoint endpoint = NetworkEndPoint.LoopbackIpv4;
        //endpoint.Port = 8000; //Make sure port isnt used (dont use 80 or 443)
    }
    public virtual void UpdateServer()
    {
        driver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMesssagePump();
    }
    public virtual void Shutdown()
    {
        driver.Dispose();
    }
    public void CheckAlive()
    {
        if (!connection.IsCreated) { Debug.Log("Lost connection to server"); connectedToServer = false; }
    }
    protected virtual void UpdateMesssagePump()
    {
        DataStreamReader stream;
        
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if(cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("connected to server");
                connectedToServer = true;
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                connection = default(NetworkConnection);
                connectedToServer = false;
                Destroy(this.gameObject);
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
            case OpCode.PLAYER_POSITION: msg = new Net_PlayerPosition(stream); updatePlayerPos(msg); break;
            case OpCode.PLAYER_ID: msg = new Net_PlayerId(stream); setId(msg.RecievedId()); break;
            case OpCode.PLAYER_ANIM: msg = new Net_PlayerAnim(stream); updateAnimData(msg); break;
            case OpCode.PLAYER_INFO: msg = new Net_PlayerInfo(stream); UpdatePlayerWeapons(msg); break;
            default:
                Debug.Log("Recieved data has no OpCode");
                break;
        }
        msg.RecievedOnClient();
    }
    public virtual void SendToServer(NetMessage msg)
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
    public void setId(int id)
    {
        sendPlayerData.enabled = true;
        sendPlayerData.PlayerId = id;
        playerList.Add(id);
    }
    public void updatePlayerPos(NetMessage msg)
    {
        int playerId = msg.RecievedId();
        if (!playerList.Contains(playerId))
        {
            playerList.Add(playerId);
            sendPlayerData.CreateNewPlayer(playerId);
        }
        Vector3 pos = msg.getPlayerPos();
        Vector3 rot = msg.getPlayerRot();
        sendPlayerData.getPositions(playerId ,pos, rot);
    }
    public void updateAnimData(NetMessage msg)
    {
        int playerId = msg.RecievedId();
        List<int> intList = msg.getPlayerAnimInts();
        List<float> floatList = msg.getPlayerAnimFloats();
        //Debug.Log("C: ID: " + intList[0] + " J: " + intList[1]+ " dJ: " + intList[2]+ " C: " + intList[3]+ " P: " + intList[4]+ " S: " + intList[5]+ " X: " + floatList[0]);

        sendPlayerData.getAnimData(intList, floatList);
    }
    public void UpdatePlayerWeapons(NetMessage msg)
    {
        int playerId = msg.RecievedId();
        string weapon = msg.getPlayerWeapon();
        sendPlayerData.updatePlayerWeapons(playerId, weapon);
    }
}
