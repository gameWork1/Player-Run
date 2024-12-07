using System;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkMatch),
    typeof(NetworkIdentity))]
public class LobbyPlayer : NetworkBehaviour
{
    private void Start()
    {
        CmdJoinRoom();
    }

    [Command(requiresAuthority = false)]
    private void CmdJoinRoom()
    {
        var networkMatch = GetComponent<NetworkMatch>();
        Guid matchId = MatchManager.singletone.JoinRoom(GetComponent<NetworkIdentity>().connectionToServer);
        networkMatch.matchId = matchId;
    }
}