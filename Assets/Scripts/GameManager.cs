using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class GameManager : NetworkBehaviour
{
    
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button quitBtn1;
    public TMP_Text theLapText;


    public GameObject gameOverUI;
      
    private void Awake()
    {
        Application.targetFrameRate = 60;

        serverBtn.onClick.AddListener(() => {
        networkManager.StartServer();});

        hostBtn.onClick.AddListener(() => {            
        networkManager.StartHost();});
         
        clientBtn.onClick.AddListener(() => {
        networkManager.StartClient();});

        quitBtn.onClick.AddListener(() => {
        Application.Quit();});
        
        quitBtn1.onClick.AddListener(() => {
        Application.Quit();});
    }

    void Start()
    {

    }

    void Update()
    {
    }

    [Rpc(SendTo.Everyone)]
    public void GameOverRpc() 
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }
    
}
