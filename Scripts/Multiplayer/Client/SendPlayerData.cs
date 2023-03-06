using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPlayerData : MonoBehaviour
{
    private float lastSend;
    private Base_Client client;

    private GameDataFile gameDataFile;
    private CharacterMovementV2 ps;
    private WeaponManager wm;

    public int PlayerId = 1;
    public bool enabled = false;
    private Transform PlayerTransform;
    public GameObject OnlinePlayerRig;
    public List<playerInfo> playerList = new List<playerInfo>();
    private float timePass;
    Transform Orientation;
    private Animation animCon;
    private string currentWeapon;
    [SerializeField] private Transform WeaponContainer;

    public class playerInfo
    {
        public int playerId;
        public Vector3 playerPos;
        public Vector3 playerRot;
        public GameObject playerRig;
        public GameObject WeaponContainer;
        public animData animData;
    }
    public class animData
    {
        public bool jump;
        public bool doubleJump;
        public bool crouch;
        public bool prone;
        public bool sStop;
        public float x;
        public float LastxyMove;
    }

    private void Start()
    {
        gameDataFile = FindObjectOfType<GameDataFile>();
        ps = gameDataFile.player.GetComponent<CharacterMovementV2>();
        wm = ps.GetComponent<WeaponManager>();
        client = gameDataFile.ActiveClient.GetComponent<Base_Client>();
        PlayerTransform = gameDataFile.player.transform;
        Orientation = gameDataFile.orientation;
        animCon = gameDataFile.player.GetComponent<Animation>();
    }

    void Update()
    {
        if (!enabled) return;
        if(Time.time - lastSend > 0.01f)
        {
            Vector3 pos = PlayerTransform.position;
            Vector3 rot = Orientation.rotation.eulerAngles;
            Net_PlayerPosition ps = new Net_PlayerPosition(PlayerId, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z);
            client.SendToServer(ps);
            client.SendToServer(sendAnimData());
            lastSend = Time.time;
            timePass = Time.time - lastSend;
            if (currentWeapon != wm.currentWeapon) 
            { 
                Net_PlayerInfo pi = new Net_PlayerInfo(PlayerId, wm.currentWeapon); 
                client.SendToServer(pi);
                currentWeapon = wm.currentWeapon;
            }
        }
        MovePlayers();
    }
    public void CreateNewPlayer(int playerId) //Adds a new player to the client game
    {
        //Check if the player already exists
        if (PlayerId == playerId) return; 

        //Create a new player 
        GameObject newPlayer = Instantiate(OnlinePlayerRig);
        newPlayer.name = "Player " + playerId.ToString();
        playerInfo playerInfo = new playerInfo();
        playerInfo.animData = new animData();
        playerInfo.playerId = playerId; playerInfo.playerRig = newPlayer;
        playerInfo.WeaponContainer = newPlayer.transform.GetChild(1).gameObject;
        playerList.Add(playerInfo);
    }
    public void getPositions(int playerId, Vector3 position, Vector3 rotation) //Reads player pos and rot from server
    {
        foreach(playerInfo player in playerList)
        {
            if(player.playerId == playerId)
            {
                Transform playerTransform = player.playerRig.transform;
                if (player.playerPos == Vector3.zero) { playerTransform.position = position; playerTransform.rotation = Quaternion.Euler(rotation); }
                player.playerPos = position;
                player.playerRot = rotation;
                return;
            }
        }
    }
    public void MovePlayers() //Applies player pos and rot to client models
    {
        foreach(playerInfo player in playerList)
        {
            Transform transform = player.playerRig.transform;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(player.playerRot), 360);
            transform.position = Vector3.MoveTowards(player.playerRig.transform.position, player.playerPos, 1); 
        }
    }
    public Net_PlayerAnim sendAnimData()
    {
        List<bool> boolList = ps.GetAnimDataBoolList();
        List<float> floatList = ps.GetAnimDataFloatList();
        //Debug.Log(floatList[1]);
        Net_PlayerAnim pa = new Net_PlayerAnim(PlayerId, boolList[0], boolList[1], boolList[2], boolList[3], boolList[4], floatList[0], floatList[1]);
        return pa;
    }

    public void getAnimData(List<int> intList, List<float> floatList)
    {
        if (intList[0] == PlayerId) return;
        foreach (playerInfo player in playerList)
        {
            if (player.playerId == intList[0])
            {
                GameObject PlayerRig = player.playerRig;
                animData anim = player.animData;
                anim.jump = convertIntToBool(intList[1]);
                anim.doubleJump = convertIntToBool(intList[2]);
                anim.crouch = convertIntToBool(intList[3]);
                anim.prone = convertIntToBool(intList[4]);
                anim.sStop = convertIntToBool(intList[5]);
                anim.x = floatList[0];
                anim.LastxyMove = floatList[1];
                Animator animcon = PlayerRig.GetComponent<Animator>();
                playAnim(anim, animcon);
                return;
            }
        }
    }
    public void playAnim(animData animData, Animator animCon)
    {
        animCon.SetBool("DoubleJump", animData.doubleJump);
        animCon.SetBool("Jump", animData.jump);
        animCon.SetBool("Crouch", animData.crouch);
        animCon.SetBool("Prone", animData.prone);
        animCon.SetFloat("X", animData.x);
        animCon.SetFloat("LastxyMove", animData.LastxyMove);

        if (animData.sStop) animCon.SetTrigger("SuddenStop");
    }
    private bool convertIntToBool(int i)
    {
        if (i == 0) return false;
        else return true;
    }

    public void updatePlayerWeapons(int playerId, string weapon)
    {
        Debug.Log("no");
        foreach (playerInfo player in playerList)
        {
            Debug.Log("1");
            if(PlayerId == player.playerId)
            {
                Debug.Log("2");
                foreach (Transform weaponModel in WeaponContainer)
                {
                    Debug.Log("3");
                    if (weaponModel.name == weapon) weaponModel.gameObject.SetActive(true);
                    else weaponModel.gameObject.SetActive(false);
                }
            }
        }
    }
}
