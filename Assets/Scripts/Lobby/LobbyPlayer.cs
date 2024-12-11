using System;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkMatch))]
public class LobbyPlayer : NetworkBehaviour
{
    private NetworkIdentity _identity;

    private void Start()
    {
        CmdJoinRoom();
    }

    [Command(requiresAuthority = false)]
    private void CmdJoinRoom()
    {
        _identity = GetComponent<NetworkIdentity>();
        if (_identity != null)
        {
            var networkMatch = GetComponent<NetworkMatch>();
            Guid matchId = MatchManager.singletone.JoinRoom(_identity.connectionToClient);
            networkMatch.matchId = matchId;
        }
        
    }
}