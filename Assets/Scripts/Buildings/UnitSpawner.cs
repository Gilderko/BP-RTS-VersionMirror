using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform spawnLocation = null;
    [SerializeField] private TextMeshProUGUI remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits = 0;
    
    [SyncVar]
    private float unitTimer = 0.0f;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }
        if (isClient)
        {
            UpdateTimerDisplay();
        }        
    }

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        health.ServerOnDie += ServerHandleDie;
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();
        health.ServerOnDie += ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (queuedUnits == maxUnitQue)
        {
            return;
        }

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetResources() < unitPrefab.GetResourceCost())
        {
            return;
        }

        queuedUnits++;

        player.AddResources(-unitPrefab.GetResourceCost());
    }

    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0)
        {
            return;
        }

        unitTimer += Time.deltaTime;

        if (unitTimer < unitSpawnDuration)
        {
            return;
        }

        GameObject spawnedUnit = Instantiate(unitPrefab.gameObject, spawnLocation.position, spawnLocation.rotation);

        NetworkServer.Spawn(spawnedUnit, connectionToClient);

        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = spawnLocation.position.y;

        UnitMovement unitMovement = spawnedUnit.GetComponent<UnitMovement>();
        unitMovement.ServerMove(spawnOffset + spawnLocation.position);

        queuedUnits--;
        unitTimer = 0.0f;
    }

    #endregion

    #region Client

    [Client]
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }

        CmdSpawnUnit();
    }

    [Client]
    private void ClientHandleQueuedUnitsUpdated(int oldAmount, int newAmount)
    {
        remainingUnitsText.text = newAmount.ToString();
    }    

    [Client]
    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;

        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress, ref progressImageVelocity, 0.1f);
        }
    }
    #endregion
}
