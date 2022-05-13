using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Takes care of navigating when clicking on the minimap.
/// </summary>
public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private float mapScale = 20f;
    [SerializeField] private float offset = -6f;

    private Transform playerCameraTransform;

#if !UNITY_SERVER

    private void Start()
    {
        if (NetworkClient.connection != null)
        {
            playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();
        }
    }

#endif

    private void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, mousePos, null, out localPos))
        {
            return;
        }

        Vector2 linInterp = new Vector2((localPos.x - minimapRect.rect.x) / minimapRect.rect.width,
            (localPos.y - minimapRect.rect.y) / minimapRect.rect.height);

        Vector3 newCameraPos = new Vector3(
            Mathf.Lerp(-mapScale, mapScale, linInterp.x),
            playerCameraTransform.position.y,
            Mathf.Lerp(-mapScale, mapScale, linInterp.y));

        playerCameraTransform.position = newCameraPos + new Vector3(0f, 0f, offset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}

