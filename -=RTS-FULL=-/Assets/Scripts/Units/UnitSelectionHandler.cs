using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();

    [SerializeField] private RectTransform unitSelectionArea = null;

    private Vector2 startPosition;
    
    private Camera mainCamera;
    private RtsPlayer rtsPlayer;

    public List<Unit> SelectedUnits { get; private set; } = new List<Unit>();// setting private setter to protect list

    void Start()
    {
        mainCamera = Camera.main;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        
    }
    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }


    void Update()
    {
        if (rtsPlayer == null)
        {
            rtsPlayer = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)// TODO need to add logic not to deselect selected unit when pressing on it
        {
            //start selection area for multi select
            StartSelectionArea();


        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)// holding sht key to select all units
        {
         foreach (Unit unit in SelectedUnits)
         {
            unit.Deselect();

         }
         SelectedUnits.Clear();

        }


        unitSelectionArea.gameObject.SetActive(true);
        
        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }
    
    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - startPosition.x;
        float areaHight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHight));// abs to get rid of *minus* in numbers
        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHight / 2);// to make anchor pos in the midle of image
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            SelectSingleUnit();
            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach(Unit unit in rtsPlayer.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit)) { continue; }
          
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);//getting screen position of a unit

            if (screenPosition.x > min.x &&//checking if unit is inside box of selector
                screenPosition.x < max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    private void SelectSingleUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

        if (!unit.hasAuthority) { return; }

        SelectedUnits.Add(unit);

        foreach (Unit selectedUnit in SelectedUnits)
        {
            selectedUnit.Select();
        }
        return;
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }
}
