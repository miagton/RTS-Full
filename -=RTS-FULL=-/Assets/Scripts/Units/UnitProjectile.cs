using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] float launchForce = 10f;
    [SerializeField] float destroyAfterSeconds = 2f;

    [SerializeField] int damageToDeal = 20;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))//checking if what we hit have NetId
        {
            if (networkIdentity.connectionToClient == connectionToClient) { return; }//if it belongs to us , return


        }

        if(other.TryGetComponent<Health>(out Health health))//if its some1s else unit  deal dmg to it
        {
            health.DealDamage(damageToDeal);
        }

        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
