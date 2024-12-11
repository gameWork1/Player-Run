using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class MatchManager : NetworkBehaviour
{
    public static MatchManager singletone;
    
    [SerializeField] private int roomLimit = 6;
    
    private Dictionary<Guid, List<NetworkConnection>> roomPlayers = new Dictionary<Guid, List<NetworkConnection>>();
    private List<Guid> rooms = new List<Guid>();

    private void Awake()
    {
        singletone = this;
        DontDestroyOnLoad(gameObject);
    }

    private Guid CreateRoom()
    {
        Guid roomId = Guid.NewGuid();
        
        roomPlayers.Add(roomId, new List<NetworkConnection>());
        rooms.Add(roomId);

        return roomId;
    }

    [Server]
    public Guid JoinRoom(NetworkConnectionToClient conn)
    {
        try
        {
            if (conn.identity == null)
            {
                (NetworkManager.singleton as CustomNetworkManager).CreatePlayerForConnection(conn);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        Guid roomId = GetRoom();
        //conn.identity.gameObject.GetComponent<NetworkMatch>().matchId = roomId;
        if (roomId != Guid.Empty)
        {
            roomPlayers[roomId].Add(conn.identity.connectionToServer);
            CheckRoomStatus(roomId);
        }
       
        return roomId;
    }

    private void CheckRoomStatus(Guid roomId)
    {
        if (roomPlayers[roomId].Count >= roomLimit)
        {
            StartCoroutine(StartGame(roomId));
        }else if (roomPlayers[roomId].Count <= 0)
        {
            rooms.Remove(roomId);
            roomPlayers.Remove(roomId);
        }
    }

    private IEnumerator StartGame(Guid roomId)
    {
        yield return new WaitForSeconds(0.2f);
        // foreach (NetworkConnection conn in NetworkServer.connections.Values)
        // {
        //     try
        //     {
        //         Debug.LogWarning(conn.identity.gameObject.name);
        //         if (conn.identity != null)
        //         {
        //             var matchNetwork = conn.identity.GetComponent<NetworkMatch>();
        //
        //             if (matchNetwork != null && matchNetwork.matchId == roomId)
        //             {
        //                 conn.Send(new SceneMessage() { sceneName = "Game", sceneOperation = SceneOperation.Normal });
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError(e);
        //     }
        //     
        // }
        NetworkManager.singleton.ServerChangeScene("Game");
    }

    private Guid GetRoom()
    {
        foreach (Guid roomId in rooms)
        {
            if (roomPlayers[roomId].Count < roomLimit)
            {
                return roomId;
            }
        }
        
        return CreateRoom();
    }
}