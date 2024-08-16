using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Collections;

public class Player : NetworkBehaviour
{
    public GameManager gameManager;
    [SerializeField] private float forwardMoveSpeed = 180f;
    [SerializeField] private float backwardMoveSpeed = 100f;
    [SerializeField] private float steerSpeed = 40f;
    [SerializeField] private Sprite[] playerskin;
    [SerializeField] GameObject obstacleToSpawn;
    [SerializeField] Transform obstaclePos;

    private Rigidbody2D _rb2D;    

    private NetworkVariable<int> lapCount = new NetworkVariable<int>(0);
    public bool[] checkpointsReached;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        checkpointsReached = new bool[3];
    }

    //set each players look and position
    public override void OnNetworkSpawn() {
        if (OwnerClientId == 0)
        { 
            SpriteRenderer SpawnSkin = GetComponent<SpriteRenderer>();
            SpawnSkin.sprite = playerskin[0];

            transform.position = new Vector3(-0.1f, 2.8f, -2f);
            transform.rotation = new Quaternion(0f, 0f, 45f, 45f);
            _rb2D = GetComponent<Rigidbody2D>();
            _rb2D.bodyType = RigidbodyType2D.Dynamic;
        }
        if (OwnerClientId == 1) 
        {
            SpriteRenderer SpawnSkin = GetComponent<SpriteRenderer>();
            SpawnSkin.sprite = playerskin[1];
            SpawnSkin.color = new Color(178f,61f,61f);

            transform.position = new Vector3(-0.1f, 3.45f, -2f);
            transform.rotation = new Quaternion(0f, 0f, 45f, 45f);
            _rb2D = GetComponent<Rigidbody2D>();
            _rb2D.bodyType = RigidbodyType2D.Dynamic;
        }

        //set all checkpoints false
        if (IsOwner) 
        {
            lapCount.Value = 0;
            for (int i = 0; i < checkpointsReached.Length; i++) 
            {
                checkpointsReached[i] = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        SendInputRpc(Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S), Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.A), Input.GetKeyDown(KeyCode.F));
    }    

    private void Update()
    {
    }
   
    //Sending inputs to server
    [Rpc(SendTo.Server)]
    private void SendInputRpc(bool w, bool s,bool d,bool a, bool f) 
    {
        float tf = Mathf.Lerp(0, steerSpeed, _rb2D.velocity.magnitude / 10);
        if (w) 
        {
            _rb2D.AddForce(transform.up * (forwardMoveSpeed * Time.deltaTime));
        }
        if (s) 
        {
            _rb2D.AddForce(transform.up * (-backwardMoveSpeed * Time.deltaTime));

        }
        if (d) 
        {
            _rb2D.AddTorque(-1 * tf * Time.deltaTime);
        }
        if (a) 
        {
            _rb2D.AddTorque(tf * Time.deltaTime);
        }
        if (f)
        {
            CreateObstacleRpc();
        }
    }
    //Checking checkpoints that are reached
    [Rpc(SendTo.Server)]
    public void CheckpointsReachedRpc(int checkpointIndex) 
    {
        if (checkpointsReached[checkpointIndex]) return;
        
            checkpointsReached[checkpointIndex] = true;        
        
        Debug.Log($"Checkpoint {checkpointIndex + 1} reached by Player {OwnerClientId}");
        if (AllCheckpointsReached()) 
        {
            lapCount.Value++;
            UpdateLapCountClientRpc(OwnerClientId, lapCount.Value + 1);
            ResetCheckpoints();
        }
    }

    //check if all checkpoints are reached
    private bool AllCheckpointsReached() 
    {
        foreach (bool reached in checkpointsReached) 
        {
            if (!reached) return false;
        }
        return true;
    }

    private void ResetCheckpoints() 
    {
        for (int i = 0; i < checkpointsReached.Length; i++)
        {
            checkpointsReached[i] = false;
        }
    }

    //Update UI
    [Rpc(SendTo.Owner)]
    public void UpdateLapCountClientRpc(ulong playerID, int newLapCount) 
    {   
        if(newLapCount >= 4) 
        {
            gameManager.GameOverRpc();
        }
        gameManager.theLapText.text = ("LAP " + newLapCount.ToString() + "/3");
        Debug.Log($"Player {playerID} now has {newLapCount} laps.");
    }

    [Rpc(SendTo.Server)]
    private void CreateObstacleRpc() 
    {
        NetworkObject ob = Instantiate(obstacleToSpawn, obstaclePos.position, Quaternion.identity).GetComponent<NetworkObject>();
        ob.Spawn();
        StartCoroutine(Despawn(ob));
    }
    
    private IEnumerator Despawn(NetworkObject ob) 
    {   
        yield return new WaitForSeconds(1f);
        ob.Despawn();
        
    }   
}