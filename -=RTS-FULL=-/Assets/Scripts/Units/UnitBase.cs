using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] Health health = null;

    public static event Action<int>  ServerOnPlayerDie;
    public static event Action<UnitBase> ServerOnBaseSpawn;
    public static event Action<UnitBase> ServerOnBaseDespawn;

    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleOnDeath;

        ServerOnBaseSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie += ServerHandleOnDeath;

        ServerOnBaseDespawn?.Invoke(this);
    }

   [Server]
    private void ServerHandleOnDeath()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);//getting conn for client for event


        NetworkServer.Destroy(gameObject);

        
    }

    #endregion


    #region Client





    #endregion
}
