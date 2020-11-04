using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour

{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Health health = null;

    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static Action<Unit> ServerOnUnitSpawned;
    public static Action<Unit> ServerOnUnitDespawned;

    public static Action<Unit> AuthorityOnUnitSpawned;
    public static Action<Unit> AuthorityOnUnitDespawned;


  public Targeter GetTargeter()
    {
        return targeter;
    }
    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    #region Server
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleOnDie;
    }
    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleOnDie;
    }
   
    [Server]
    private void ServerHandleOnDie()//method called when Death event is invoked on server=> for setup custom logic for units/buildings etc
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
       // if( !hasAuthority) { return; }//checks if client is not a server or if he has authority dont need it in onstartAuthority
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
        AuthorityOnUnitDespawned?.Invoke(this);
    }


    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }
        
            onSelected?.Invoke();// ? checking if the event isnt null
    }
    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }

    

    #endregion

}
