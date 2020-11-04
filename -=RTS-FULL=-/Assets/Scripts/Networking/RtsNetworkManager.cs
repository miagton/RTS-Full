using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RtsNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
     
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

    public override void OnServerSceneChanged(string sceneName)//when server changes scene 
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))//if scene name sart with 
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);//spawn GameOverHandler
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
