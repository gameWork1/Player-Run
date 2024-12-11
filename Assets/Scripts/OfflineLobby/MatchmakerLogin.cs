using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakerLogin : MonoBehaviour
{
    [SerializeField] private bool isServer;
    
    #if isServer != true
        [SerializeField] private Button playBtn;
    #endif
    
    private void Start()
    {
        #if isServer == true
        NetworkManager.singleton.StartServer();  
        #endif
        #if isServer != true
        playBtn.onClick.AddListener(() => Join());
        #endif
        
    }

    public void Join()
    {
        NetworkManager.singleton.StartClient();
        Invoke(nameof(CheckServerConnection), 2f);
    }
    
    private void CheckServerConnection()
    {
        Debug.Log(123);
        if (!NetworkClient.isConnected)
        {
            Debug.Log("No server found. Starting as Host...");
            NetworkManager.singleton.StopClient(); // Ensure client stops before switching
            NetworkManager.singleton.StartHost();
        }
    }
}
