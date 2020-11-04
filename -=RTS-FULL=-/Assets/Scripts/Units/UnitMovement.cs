using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : NetworkBehaviour
{
   [SerializeField] private NavMeshAgent agent=null;
   [SerializeField] private Targeter targeter = null;

    [SerializeField] private float chaseRange = 10f;



    #region Server
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }



    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange*chaseRange)// not using vector3.distance to optimize it 
            {
                //chase
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                //stop chasing
                agent.ResetPath();
            }
            
            return;
        }
        
        if(!agent.hasPath) { return; }
        if (agent.remainingDistance > agent.stoppingDistance) { return; }
        agent.ResetPath();
    }

    [Command]
   public void CmdMove(Vector3 position)
   {
     if(!NavMesh.SamplePosition(position,out NavMeshHit hit,1f,NavMesh.AllAreas)){return;}//checking if mouse pos is on navmesh

        targeter.ClearTarget();//clearing traget of a unit if Move command is ordered
        agent.SetDestination(hit.position);

   }

    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }
    #endregion

}
