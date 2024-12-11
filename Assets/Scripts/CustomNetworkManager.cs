using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameObject gamePlayer;
    private bool isGameStarted;
    private Dictionary<NetworkConnectionToClient, Guid> 
        players = new(),
        oldPlayers = new();
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            Debug.LogError("Player already exists for this connection.");
            return;
        }

        CreatePlayerForConnection(conn);
    }

    public void CreatePlayerForConnection(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        if (newSceneName == "Game" && !isGameStarted)
        {
            foreach (var player in NetworkServer.connections.Values)
            {
                if (player.identity != null && !oldPlayers.ContainsKey(player))
                {
                    var matchId = player.identity.GetComponent<NetworkMatch>().matchId; 
                    players.Add(player, matchId);
                    oldPlayers.Add(player, matchId);
                }
            }
            isGameStarted = true;
            base.OnServerChangeScene(newSceneName);
        }else if (newSceneName != "Game") isGameStarted = false;
        if(newSceneName != "Game")
            base.OnServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (sceneName == "Game")
        {
            foreach (var conn in NetworkServer.connections.Values)
            {
                try
                {
                    if (players.ContainsKey(conn) && !oldPlayers.ContainsKey(conn))
                    {
                        var player = Instantiate(gamePlayer);
                        
                        player.GetComponent<NetworkMatch>().matchId = players[conn];

                        NetworkServer.AddPlayerForConnection(conn, player);
                    }
                        
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                players.Remove(conn);
            }
        }
        
    }
}
