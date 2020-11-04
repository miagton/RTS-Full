using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{

    public static event Action ServerOnGameOver;// event for server side
    public static event Action<string> ClientOnGameOver;// event for teh game over scenario on client
    
    private List<UnitBase> Bases = new List<UnitBase>();
    
    #region Server

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawn += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawn += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawn -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawn -= ServerHandleBaseDespawned;
    }
    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        Bases.Add(unitBase);
    }
    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        Bases.Remove(unitBase);

        if(Bases.Count!=1) { return; }

        int playerId = Bases[0].connectionToClient.connectionId;


        RpcGameOver($"Player {playerId}");

        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
