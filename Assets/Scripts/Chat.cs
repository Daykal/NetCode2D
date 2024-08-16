using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;

public class Chat : NetworkBehaviour
{
    [SerializeField] private TMP_Text chatText;
    private ulong playerID;
    void Start()
    {    
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerID = NetworkManager.Singleton.LocalClientId;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SubmitMsgRpc("GoodJob!", playerID);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            SubmitMsgRpc("Watch and learn!", playerID);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            SubmitMsgRpc("Hahaha", playerID);
        }                 
    }

    [Rpc(SendTo.Server)]
    private void SubmitMsgRpc(FixedString128Bytes msg, ulong playerid) 
    {
        UpdateMsgRpc(msg, playerid);
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateMsgRpc(FixedString128Bytes msg, ulong playerid) 
    {
        chatText.text = ("Chat: Player ID ") + playerid + " " + msg.ToString();
    }
}
