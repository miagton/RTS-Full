using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using System;
using Telepathy;

public class UnitSpawner : NetworkBehaviour,IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private Health health = null;



    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleOnDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleOnDie;
    }

    [Server]
    private void ServerHandleOnDie()
    {
       // transform.position = Vector3.MoveTowards(transform.position, transform.position - Vector3.up * 15, 20f * Time.deltaTime);
       // NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position,unitSpawnPoint.rotation);

        // for spawning obj in Mirror(network , so obj can be seen on clients from list of registrated prefabs in NetworkManager
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)//unity inbuild interface for checking if obj was clicked on
    {
      if(eventData.button != PointerEventData.InputButton.Left) { return; }//preventing from working with left mouse button

      if (!hasAuthority) { return; }//checking if client has authority over this gameobj
      
      CmdSpawnUnit();//command to server to spawn obj
    }

    #endregion
}
