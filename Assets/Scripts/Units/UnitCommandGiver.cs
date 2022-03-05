using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

#if !UNITY_SERVER

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
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return;
        }

        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if (target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }

            TryTarget(target);
        }
        else
        {
            TryMove(hit.point);
        }
    }

#endif

    private void ClientHandleGameOver(string obj)
    {
        enabled = false;
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    public void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
}
