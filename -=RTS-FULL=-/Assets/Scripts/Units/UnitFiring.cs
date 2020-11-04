using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] Transform[] projectileSpawnPoints = null;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();


        if (target == null) { return; }
        
        if (!CanFireAtTarget()) { return; }
        //geting vector pointing towards teh unit
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        //implementing rotating towards target with a speed and frame rate independent
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if(Time.time > (1/attackSpeed ) + lastFireTime)
        {
            // we can now fire
            
           for(int i = 0; i < projectileSpawnPoints.Length; i++)
            {

                Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoints[i].position);
                GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoints[i].position, projectileRotation);

                NetworkServer.Spawn(projectileInstance, connectionToClient);
            }
           

            lastFireTime = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget()
    {
        //same distance check as in Movement.cs
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= attackRange * attackRange;
    }
}
