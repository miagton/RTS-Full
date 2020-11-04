using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler selectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();// struckt reference

    private Camera mainCamera = null;

    private void Start()
    {
        mainCamera = Camera.main;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; } ;

        if(hit.collider.TryGetComponent<Targetable>(out Targetable target))//checking if we clicked Targetable unit
        {
            if (target.hasAuthority)// if client own this unit => move to it
            {
                TryMove(hit.point);
                return;
            }
            TryTarget(target);// if dont own this unit => target it
            return;
        }
        TryMove(hit.point);//else jsut move
    }

    private void TryMove(Vector3 point)// trying to move selected unit to point hited with raycast
    {
        foreach(Unit unit in selectionHandler.SelectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }

    private void TryTarget(Targetable target)// trying to move selected unit to point hited with raycast
    {
        foreach (Unit unit in selectionHandler.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }
    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }
}
