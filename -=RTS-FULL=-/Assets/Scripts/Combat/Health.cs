using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxhealth = 100;
    [SerializeField] Targetable targetable = null;

    [SyncVar(hook =nameof(HandleHealthUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    #region Server
   
    public override void OnStartServer()
    {
        currentHealth = maxhealth;
        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    public void DealDamage(int damage)
    {
        if (currentHealth == 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth != 0) { return; }

         // TOD disabling OBJ to be targetable after its health reaches 0

        ServerOnDie?.Invoke();//event called on server when health of OBJ reaches 0

        
    }
    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId) { return; }//if a player who dies is not this player=> return

        DealDamage(currentHealth);
    }

    #endregion


    #region Client
    private void HandleHealthUpdated(int oldHealth,int newhealth)
    {
        ClientOnHealthUpdated?.Invoke(newhealth, maxhealth);
    }

    #endregion
}
