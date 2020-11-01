using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;




public class RtsNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    
    //adding new logic
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        //spawning gameobj on server for a new player added
      GameObject unitSpawnerInstance=  Instantiate(
          unitSpawnerPrefab,
          conn.identity.transform.position,
          conn.identity.transform.rotation);

        //spawn same obj in network for each player
        NetworkServer.Spawn(unitSpawnerInstance, conn);

    }
}
