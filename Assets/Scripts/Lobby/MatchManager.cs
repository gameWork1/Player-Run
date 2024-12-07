using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class MatchManager : NetworkBehaviour
{
    public static MatchManager singletone;
    
    [SerializeField] private int roomLimit = 6;
    
    private Dictionary<Guid, List<NetworkConnection>> roomPlayers = new();
    private List<Guid> rooms = new();

    private void Awake()
    {
        singletone = this;
    }

    private Guid CreateRoom()
    {
        Guid roomId = Guid.NewGuid();
        Debug.Log(roomId);
        
        roomPlayers.Add(roomId, new());
        rooms.Add(roomId);

        return roomId;
    }

    public Guid JoinRoom(NetworkConnection conn)
    {
        Guid roomId = GetRoom();
        conn.identity.gameObject.GetComponent<NetworkMatch>().matchId = roomId;
        roomPlayers[roomId].Add(conn);
        CheckRoomStatus(roomId);
        return roomId;
    }

    private void CheckRoomStatus(Guid roomId)
    {
        if (roomPlayers[roomId].Count >= roomLimit)
        {
            StartGame(roomId);
        }else if (roomPlayers[roomId].Count <= 0)
        {
            rooms.Remove(roomId);
            roomPlayers.Remove(roomId);
        }
    }

    private void StartGame(Guid roomId)
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            var matchNetwork = conn.identity.GetComponent<NetworkMatch>();

            if (matchNetwork != null && matchNetwork.matchId == roomId)
            {
                conn.Send(new SceneMessage() { sceneName = "Game", sceneOperation = SceneOperation.Normal});
            }
        }
    }

    private Guid GetRoom()
    {
        if (rooms.Count > 0)
        {
            foreach (Guid roomId in rooms)
            {
                if (roomPlayers[roomId].Count < roomLimit)
                {
                    return roomId;
                }
            }

        }
        
        return CreateRoom();
    }
}