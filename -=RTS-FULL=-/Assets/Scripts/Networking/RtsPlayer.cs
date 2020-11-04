using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsPlayer : NetworkBehaviour
{
   [SerializeField] private List<Unit> myUnits = new List<Unit>();

    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    #region Server
    public override void OnStartServer()//subscribing to events
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawn;

        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawn;
    }

    public override void OnStopServer()//unsubscribing from events
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawn;

        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawn;
    }

    private void ServerHandleUnitSpawn(Unit unit)//adding unit to this players unit list if it's HES unit
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawn(Unit unit)//removing unit from list
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myUnits.Remove(unit);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawn;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawn;

    }
    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawn;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawn;

    }
    private void AuthorityHandleUnitSpawn(Unit unit)
    {
        
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawn(Unit unit)
    {
        
        myUnits.Remove(unit);
    }



    #endregion
}
