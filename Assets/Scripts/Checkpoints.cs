using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public int checkpointIndex;
    void Start()
    {
    }

    void Update()
    {        
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsOwner)
        {
            Player player = networkObject.GetComponent<Player>();
            if (player != null)
            {
                player.CheckpointsReachedRpc(checkpointIndex);
            }
           
        }
    }
}
